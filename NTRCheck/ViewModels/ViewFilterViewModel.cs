using LogLib;
using NORMLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;
using ViewModelLib.Attributes;

namespace NTRCheck.ViewModels
{
	public class ViewFilterViewModel : ViewModel<ViewFilter>
	{
		[Editable(IsMandatory = true,Header ="Column", IsEditable = false,  AllowedValuesPath = "FilterableColumns")]
		public string ColumnName
		{
			get { return Model.ColumnName; }
			set { Model.ColumnName= value; OnPropertyChanged();OnPropertyChanged("Column"); }
		}

		[Editable(IsMandatory = true,  Header ="Operator", IsEditable =false,DisplayMemberPath = "Description", SelectedValuePath ="ID",AllowedValuesPath = "ViewFilterOperators")]
		public int? OperatorID
		{
			get { return Model.OperatorID; }
			set { Model.OperatorID = value; OnPropertyChanged();OnPropertyChanged("Operator"); }
		}

		[Editable(IsMandatory = true, Header = "Value")]
		public string Value
		{
			get { return Model.Value; }
			set { Model.Value = value; OnPropertyChanged(); }
		}

		private Cache<ViewFilterOperator> _operator;
		public ViewFilterOperator Operator
		{
			get { return _operator.Value; }
		}

		private Cache<IColumn> column;
		public IColumn Column
		{
			get { return column.Value; }
		}

		public ViewFilterViewModel(ILogger Logger) : base(Logger)
		{
			_operator = new Cache<ViewFilterOperator>(() => { return ViewFilterViewModelCollection.viewFilterOperators.FirstOrDefault(item => item.ID == this.OperatorID.Value); },this,"OperatorID");
			column = new Cache<IColumn>(() => { return Table<CVS>.Columns.FirstOrDefault(item=>item.Name==ColumnName); }, this, "ColumnName");
		}

		private object OnConvertFilterValue(object Value)
		{
			Type targetType;
			
			targetType = Column.ColumnType;

			if (Value == null) return null;
			if (Value.GetType()==targetType) return Value;

			if (targetType.IsGenericType) targetType = targetType.GenericTypeArguments[0];

			try
			{
				return Convert.ChangeType(Value, targetType);
			}
			catch
			{
				return null;
			}
		}


		public bool Filter(CVS Item)
		{
			IComparable comparable;
			object value,filterValue;

			value = Column.GetValue(Item);

			comparable = value as IComparable;
			if (comparable == null) return false;

			filterValue = OnConvertFilterValue(Value);

			switch(OperatorID)
			{
				case 0: // =
					if (comparable.CompareTo(filterValue) != 0) return false;
					break;
				case 1: // !=
					if (comparable.CompareTo(filterValue) == 0) return false;
					break;
				case 2: // like
					if (!Item.ToString().Contains(Value.ToString())) return false;
					break;
				case 3: // <
					if (comparable.CompareTo(filterValue) >= 0) return false;
					break;
				case 4: // >
					if (comparable.CompareTo(filterValue) <= 0) return false;
					break;
				case 5: // <=
					if (comparable.CompareTo(filterValue) > 0) return false;
					break;
				case 6: // >=
					if (comparable.CompareTo(filterValue) < 0) return false;
					break;
				default:return false;
			}

			return true;
		}


	}
}
