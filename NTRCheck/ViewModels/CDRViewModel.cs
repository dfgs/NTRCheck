using LogLib;
using NORMLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace NTRCheck.ViewModels
{
	public  class CDRViewModel : CVSViewModel
	{
		
		public VOXViewModelCollection VOXs
		{
			get;
			private set;
		}

		public CDRViewModel(ILogger Logger) : base(Logger)
		{
			VOXs = new VOXViewModelCollection(Logger);
		}

		protected override async Task OnLoadedAsync(CVS Model)
		{
			await base.OnLoadedAsync(Model);
			await VOXs.LoadAsync(this);
		}


	}
}
