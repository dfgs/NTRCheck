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
	public class CDRViewModelCollection : CVSViewModelCollection<CDRViewModel>
	{


		public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Statuses), typeof(CDRViewModelCollection));
		public Statuses Status
		{
			get { return (Statuses)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}


		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(CDRViewModelCollection));
		public double Progress
		{
			get { return (double)GetValue(ProgressProperty); }
			private set { SetValue(ProgressProperty, value); }
		}


		

		public CDRViewModelCollection(ILogger Logger) : base(Logger)
		{
		}

		protected override CDRViewModel OnCreateViewModel(CVS Model)
		{
			return new CDRViewModel(Logger);
		}

		public async Task LoadAsync(CaseViewModel Case)
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
		}

		public async Task StopAsync()
		{
			Status = Statuses.Stopping;
			await Task.Yield();
		}
		public bool CanClear()
		{
			return IsLoaded && (!IsLoading) && (Count > 0);
		}
		public async Task ClearAsync(CaseViewModel Case, Func<bool> ClearCallBack)
		{
			IDelete<CVS> query;
			Filter filter;

			try
			{
				IsLoading = true;
				if (ClearCallBack != null)
				{
					if (!ClearCallBack()) return;
				}
				filter = CVS.CaseIDColumn.IsEqualTo(Case.CaseID.Value).And(CVS.CVSTYPColumn.IsEqualTo("VOX"));
				query = new Delete<CVS>().Where(filter);
				await Task.Run(() => { this.Dispatcher.Invoke(() => { Server.ExecuteNonQuery(query); }); });
				filter = CVS.CaseIDColumn.IsEqualTo(Case.CaseID.Value).And(CVS.CVSTYPColumn.IsEqualTo("CDR"));
				query = new Delete<CVS>().Where(filter);
				await Task.Run(() => { this.Dispatcher.Invoke(() => { Server.ExecuteNonQuery(query); }); });

				Items.Clear();
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

		public async Task<TimeSpan> ImportAsync(CaseViewModel Case, DateTime StartDate, DateTime EndDate)
		{
			IEnumerable<CVS> results;
			CDRViewModel viewModel;
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

				for (int day=0;day<totalDays;day++)
				{
					if (Status != Statuses.Running) break;

					filter = new AndFilter(CVS.CVSSDTColumn.IsGreaterOrEqualsThan(StartDate.AddDays(day)), CVS.CVSSDTColumn.IsLowerThan(StartDate.AddDays(day+1)));
					query = new Select<CVS>( CVS.CVSKEYColumn, CVS.CVSUSRColumn, CVS.CVSCHNColumn, CVS.UtcStartTimeColumn, CVS.UtcEndTimeColumn, CVS.CVSSDTColumn, CVS.CVSEDTColumn, CVS.CVSTYPColumn, CVS.CVSC05Column).Where(filter);

					results = await Task.Run<IEnumerable<CVS>>(() => {
						return this.Dispatcher.Invoke <IEnumerable<CVS>>(() => { return Case.CreateServer().Execute<CVS>(query); });
						
					});

					foreach (CVS model in results)
					{
						if (Status != Statuses.Running) break;
						if (this.FirstOrDefault(item => item.CVSKEY.Value == model.CVSKEY.Value) != null) continue;
						
						model.CaseID = Case.CaseID;
						viewModel = OnCreateViewModel(model);
						await viewModel.LoadAsync(model);
						await AddAsync(viewModel, null);
						await viewModel.LoadAsync(model); // reload this viewModel in order to have a valid CVSID and VOX table
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
				while(index<Count)
				{
					if (Status != Statuses.Running) break;
					Progress = 100 * index / Count;

					vox = this[index];
					if (vox.CVSTYP == "CDR")
					{
						index++;
						continue;
					}
					cdr = this.FirstOrDefault(item =>(item.CVSTYP=="CDR") && (item.CVSCHN.Value == vox.CVSCHN.Value) && (vox.CVSSDT <= item.CVSEDT) && (vox.CVSEDT >= item.CVSSDT));
					if (cdr == null)
					{
						index++;
						continue;
					}

					vox.ParentCVSID = cdr.CVSID;
					await Task.Run( () => this.Dispatcher.Invoke( () => Server.ExecuteNonQuery(new Update<CVS>(vox.Model).Where(CVS.CVSIDColumn.IsEqualTo(vox.CVSID.Value) ) ) ) );

					Items.RemoveAt(index);
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,vox,index));

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
