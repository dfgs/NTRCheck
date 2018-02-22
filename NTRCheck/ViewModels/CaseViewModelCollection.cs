﻿using LogLib;
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
	public class CaseViewModelCollection : BaseViewModelCollection<CaseViewModel, Case>
	{
		public CaseViewModelCollection(ILogger Logger, IServer Server) : base(Logger,Server )
		{
		
		}

		protected override CaseViewModel OnCreateViewModel(Case Model)
		{
			return new CaseViewModel(Logger);
		}

		protected override Task OnClearModelAsync()
		{
			throw new NotImplementedException();
		}

	}
}