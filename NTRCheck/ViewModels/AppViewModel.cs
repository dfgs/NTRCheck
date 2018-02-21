using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LogLib;
using NORMLib;
using ViewModelLib;

namespace NTRCheck.ViewModels
{
	public class AppViewModel : ViewModel<string>
	{
		public CaseViewModelCollection Cases
		{
			get;
			private set;
		}

		public CVSViewModelCollection CVSs
		{
			get;
			private set;
		}


		public static readonly DependencyProperty CurrentCaseProperty = DependencyProperty.Register("CurrentCase", typeof(CaseViewModel), typeof(AppViewModel));
		public CaseViewModel CurrentCase
		{
			get { return (CaseViewModel)GetValue(CurrentCaseProperty); }
			set { SetValue(CurrentCaseProperty, value); }
		}


		public AppViewModel(ILogger Logger, IServer Server) : base(Logger)
		{
			CurrentCase = null;
			Cases = new CaseViewModelCollection(Logger,Server);
			CVSs = new CVSViewModelCollection(Logger,Server);
		}

		protected override async Task OnLoadedAsync(string Model)
		{
			await Cases.LoadAsync();
		}


	}
}
