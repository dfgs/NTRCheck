using LogLib;
using NORMLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace NTRCheck.ViewModels
{
	public class CVSViewModelCollection : BaseViewModelCollection<CVSViewModel, CVS>
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
			private set { SetValue(ProgressProperty, value); }
		}


		public CaseViewModel Case
		{
			get;
			private set;
		}

		public CVSViewModelCollection(ILogger Logger, IServer Server, CaseViewModel Case) : base(Logger,Server )
		{
			this.Case = Case;
		}

		protected override CVSViewModel OnCreateViewModel(CVS Model)
		{
			return new CVSViewModel(Logger);
		}

		public override async Task LoadAsync()
		{
			IEnumerable<CVS> model;

			try
			{
				model = Server.Execute<CVS>(new Select<CVS>().Where(CVS.CaseIDColumn.IsEqualTo(Case.CaseID.Value) ) );
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

		protected override async Task OnClearModelAsync()
		{
			int caseID;

			caseID = Dispatcher.Invoke<int>(() => { return Case.CaseID.Value; });
			await Task.Run(() => Server.ExecuteNonQuery(new Delete<CVS>().Where(CVS.CaseIDColumn.IsEqualTo(caseID))));
		}

		public async Task<TimeSpan> ImportAsync(DateTime StartDate, DateTime EndDate)
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

				for (int day=0;day<totalDays;day++)
				{
					if (Status != Statuses.Running) break;

					filter = new AndFilter(CVS.CVSSDTColumn.IsGreaterOrEqualsThan(StartDate.AddDays(day)), CVS.CVSSDTColumn.IsLowerThan(StartDate.AddDays(day+1)));
					query = new Select<CVS>(CVS.CVSKEYColumn, CVS.CVSUSRColumn, CVS.CVSCHNColumn, CVS.UtcStartTimeColumn, CVS.UtcEndTimeColumn, CVS.CVSSDTColumn, CVS.CVSEDTColumn, CVS.CVSTYPColumn, CVS.CVSC05Column).Where(filter);

					results = await Task.Run<IEnumerable<CVS>>(() => {
						return this.Dispatcher.Invoke <IEnumerable<CVS>>(() => { return Case.Server.Execute<CVS>(query); });
						
					});

					foreach (CVS model in results)
					{
						if (Status != Statuses.Running) break;
						if (this.FirstOrDefault(item => item.CVSKEY.Value == model.CVSKEY.Value) != null) continue;
						
						model.CaseID = Case.CaseID;
						viewModel = new CVSViewModel(Logger);
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


	}
}
