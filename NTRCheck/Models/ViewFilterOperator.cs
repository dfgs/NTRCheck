using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTRCheck.Models
{
	public struct ViewFilterOperator
	{
		

		public int ID
		{
			get;
			private set;
		}
		public string Description
		{
			get;
			private set;
		}
		public char Symbol
		{
			get;
			private set;
		}

		public ViewFilterOperator(int ID,string Description,char Symbol)
		{
			this.ID = ID;this.Description = Description;this.Symbol = Symbol;
		}
	}

	
	
}
