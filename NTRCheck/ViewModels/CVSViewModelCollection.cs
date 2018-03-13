using LogLib;
using NORMLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace NTRCheck.ViewModels
{
	public class CVSViewModelCollection : BaseViewModelCollection<CVSViewModel,CVS>
	{


		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(CVSViewModelCollection));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}


		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(CVSViewModelCollection));
		public double Progress
		{
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}




		public CVSViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override CVSViewModel OnCreateViewModel(CVS Model)
		{
			return new CVSViewModel(Logger);
		}

		/*public async Task LoadAsync(CaseViewModel Case)
		{
			IEnumerable<CVS> model;

			try
			{
				model = Server.Execute<CVS>(new Select<CVS>().Where(
					new AndFilter(CVS.CaseIDColumn.IsEqualTo(Case.CaseID.Value), CVS.ParentCVSIDColumn.IsEqualTo(null)))
					);
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			await LoadAsync(model);
		}*/

		public async Task StopAsync()
		{
			Status = Statuses.Stopping;
			await Task.Yield();
		}
		public bool CanClear()
		{
			return IsLoaded && (!IsLoading) && (Count > 0);
		}

		public async Task ClearAsync(Func<bool> ClearCallBack)
		{
			try
			{
				IsLoading = true;
				if (ClearCallBack != null)
				{
					if (!ClearCallBack()) return;
				}
				await Dispatcher.InvokeAsync(() =>
				{
					((IList<CVS>)Model).Clear();
					Items.Clear();
				}
				);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
				IsLoading = false;
			}

		}

		public async Task<TimeSpan> ImportAsync(IServer Server, DateTime StartDate, DateTime EndDate)
		{
			IEnumerable<CVS> results;
			CVSViewModel viewModel;
			ISelect<CVS> query;
			Filter filter;
			double totalDays;
			DateTime start;


			try
			{
				start = DateTime.Now;
				Status = Statuses.Running;
				Progress = 0;

				totalDays = (EndDate - StartDate).TotalDays + 1;

				for (int day = 0; day < totalDays; day++)
				{
					if (Status != Statuses.Running) break;

					filter = new AndFilter(CVS.CVSSDTColumn.IsGreaterOrEqualsThan(StartDate.AddDays(day)), CVS.CVSSDTColumn.IsLowerThan(StartDate.AddDays(day + 1)));
					query = new Select<CVS>(CVS.CVSKEYColumn, CVS.CVSUSRColumn, CVS.CVSCHNColumn, CVS.UtcStartTimeColumn, CVS.UtcEndTimeColumn, CVS.CVSSDTColumn, CVS.CVSEDTColumn, CVS.CVSTYPColumn, CVS.CVSC05Column).Where(filter);

					results = await Dispatcher.InvokeAsync<IEnumerable<CVS>>(() => {
						return Server.Execute<CVS>(query);
					});

					foreach (CVS model in results)
					{
						if (Status != Statuses.Running) break;
						if (this.FirstOrDefault(item => item.CVSKEY.Value == model.CVSKEY.Value) != null) continue;

						viewModel = OnCreateViewModel(model);
						await viewModel.LoadAsync(model);
						await AddAsync(viewModel, null);
					}

					Progress = 100 * day / totalDays;
				}


			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
				Status = Statuses.Idle;
			}

			return DateTime.Now - start;
		}


		public async Task<TimeSpan> AssociateAsync()
		{
			DateTime start;
			int index;
			CVSViewModel cdr,vox;

			try
			{
				start = DateTime.Now;
				Status = Statuses.Running;
				Progress = 0;


				index = 0;
				while (index < Count)
				{
					if (Status != Statuses.Running) break;
					Progress = 100 * index / Count;

					vox = this[index];
					vox.Status = CVSStatuses.Unknown;

					if (vox.CVSTYP == "CDR")
					{
						index++;
						continue;
					}
					cdr = this.FirstOrDefault(item => (item.CVSTYP == "CDR") && (item.CVSCHN.Value == vox.CVSCHN.Value) && (vox.CVSSDT <= item.CVSEDT) && (vox.CVSEDT >= item.CVSSDT));
					if (cdr == null)
					{
						index++;
						continue;
					}


					if (cdr.VOXs.FirstOrDefault(item => item.CVSKEY == vox.CVSKEY) == null)
					{
						await cdr.VOXs.AddAsync(vox, null);
					}
					Items.RemoveAt(index);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, vox, index));
				}


			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
				Status = Statuses.Idle;
			}

			return DateTime.Now - start;
		}


		public async Task<TimeSpan> AnalyseAsync()
		{
			DateTime start;
			CVSViewModel  cdr;

			try
			{
				start = DateTime.Now;
				Status = Statuses.Running;
				Progress = 0;


				for(int index=0;index<Count;index++)
				{
					if (Status != Statuses.Running) break;
					Progress = 100 * index / Count;

					cdr = this[index];
					if (cdr.CVSTYP == "CDR")
					{
						if (cdr.VOXs.Count == 0)
						{
							cdr.Status = CVSStatuses.Unknown;
							continue;
						}

						cdr.Status = CVSStatuses.Valid;
						foreach (CVSViewModel vox in cdr.VOXs)
						{
							if (vox.CVSUSR!=cdr.CVSUSR)
							{
								vox.Status = CVSStatuses.Invalid;
								cdr.Status = CVSStatuses.Invalid;
							}
							else
							{
								vox.Status = CVSStatuses.Valid;
							}
						}

						
					}
					else
					{
						cdr.Status = CVSStatuses.Orphaned;
					}

					await Task.Yield();
				}


			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
				Status = Statuses.Idle;
			}

			return DateTime.Now - start;
		}





	}

}
