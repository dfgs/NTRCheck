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
	public abstract class CVSViewModelCollection<ViewModelType> : BaseViewModelCollection<ViewModelType, CVS>
		where ViewModelType:CVSViewModel
	{
		

		public CVSViewModelCollection(ILogger Logger) : base(Logger )
		{
			//this.Case = Case;
		}

		


		



	}
}
