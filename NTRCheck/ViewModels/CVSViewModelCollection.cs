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
	public class CVSViewModelCollection : BaseViewModelCollection<CVSViewModel, CVS>
	{
		public CVSViewModelCollection(ILogger Logger, IServer Server) : base(Logger,Server )
		{
		
		}

		protected override CVSViewModel OnCreateViewModel(CVS Model)
		{
			return new CVSViewModel(Logger);
		}
		public async Task LoadAsync(int CaseID)
		{
			IEnumerable<CVS> model;

			try
			{
				model = Server.Execute<CVS>(new Select<CVS>().Where(CVS.CaseIDColumn.IsEqualTo(CaseID) ) );
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			await LoadAsync(model);
		}

		public async Task LoadAsync(DateTime startDate, DateTime endDate)
		{
			throw new NotImplementedException();
		}


	}
}
