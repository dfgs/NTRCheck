using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NTRCheck.Models
{
	public class ViewFilter
	{
		public static readonly Column<ViewFilter, int?> ViewFilterIDColumn = new Column<ViewFilter, int?>() { DefaultValue = 0, IsPrimaryKey = true };
		[XmlIgnore]
		public int? ViewFilterID
		{
			get { return ViewFilterIDColumn.GetValue(this); }
			set { ViewFilterIDColumn.SetValue(this, value); }
		}


		public static readonly Column<ViewFilter, string> ColumnNameColumn = new Column<ViewFilter, string>();
		[XmlAttribute]
		public string ColumnName
		{
			get { return ColumnNameColumn.GetValue(this); }
			set { ColumnNameColumn.SetValue(this, value); }
		}



		public static readonly Column<ViewFilter, int?> OperatorIDColumn = new Column<ViewFilter, int?>();
		[XmlElement]
		public int? OperatorID
		{
			get { return OperatorIDColumn.GetValue(this); }
			set { OperatorIDColumn.SetValue(this, value); }
		}
		

		public static readonly Column<ViewFilter, string> ValueColumn = new Column<ViewFilter, string>();
		[XmlAttribute]
		public string Value
		{
			get { return ValueColumn.GetValue(this); }
			set { ValueColumn.SetValue(this, value); }
		}





		public ViewFilter()
		{
		}

	}



}
