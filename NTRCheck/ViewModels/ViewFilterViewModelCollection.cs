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
	
	public class ViewFilterViewModelCollection : BaseViewModelCollection<ViewFilterViewModel, ViewFilter>
	{
		public static readonly ViewFilterOperator[] viewFilterOperators = new ViewFilterOperator[]
		{
			new ViewFilterOperator(0,"Is equal to",'='),
			new ViewFilterOperator(1,"Is not equal to",'≠'),
			new ViewFilterOperator(2,"Is like",'≈'),
			new ViewFilterOperator(3,"Is lower than",'<'),
			new ViewFilterOperator(4,"Is greater than",'>'),
			new ViewFilterOperator(5,"Is lower or equal than",'≤'),
			new ViewFilterOperator(6,"Is greater or equal than",'≥')
		};
		public ViewFilterOperator[] ViewFilterOperators
		{
			get { return viewFilterOperators; }
		}

		public static readonly IEnumerable<string> filterableColumns;
		public IEnumerable<string> FilterableColumns
		{
			get { return filterableColumns; }
		}

		static ViewFilterViewModelCollection()
		{
			filterableColumns = Table<CVS>.Columns.Select(item => item.Name);
		}

		public ViewFilterViewModelCollection(ILogger Logger) : base(Logger)
		{
			
		}

		protected override ViewFilterViewModel OnCreateViewModel(ViewFilter Model)
		{
			return new ViewFilterViewModel(Logger);
		}
	}
}
