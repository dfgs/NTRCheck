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
	public class VOXViewModelCollection : CVSViewModelCollection<VOXViewModel>
	{

		

		public VOXViewModelCollection(ILogger Logger) : base(Logger)
		{
			
		}

		protected override VOXViewModel OnCreateViewModel(CVS Model)
		{
			return new VOXViewModel(Logger);
		}

		public async Task LoadAsync(CDRViewModel CDR)
		{
			IEnumerable<CVS> model;

			try
			{
				if (!CDR.CVSID.HasValue) return;
				model = Server.Execute<CVS>(new Select<CVS>().Where(
					new AndFilter(CVS.CaseIDColumn.IsEqualTo(CDR.CaseID.Value), CVS.ParentCVSIDColumn.IsEqualTo(CDR.CVSID.Value)))
					);
							
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			await LoadAsync(model);
		}

		



	}
}
