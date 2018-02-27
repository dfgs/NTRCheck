using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NTRCheck.Models
{
	public class Case
	{

		public static readonly Column<Case, int?> CaseIDColumn = new Column<Case, int?>() { DefaultValue=0,IsPrimaryKey=true };
		[XmlIgnore]
		public int? CaseID
		{
			get { return CaseIDColumn.GetValue(this); }
			set { CaseIDColumn.SetValue(this, value); }
		}



		public static readonly Column<Case, string> DescriptionColumn = new Column<Case, string>() { DefaultValue = "New case" };
		[XmlAttribute]
		public string Description
		{
			get { return DescriptionColumn.GetValue(this); }
			set { DescriptionColumn.SetValue(this, value); }
		}


		public static readonly Column<Case, string> CoreHostNameColumn = new Column<Case, string>() { DefaultValue = "localhost" };
		[XmlAttribute]
		public string CoreHostName
		{
			get { return CoreHostNameColumn.GetValue(this); }
			set { CoreHostNameColumn.SetValue(this, value); }
		}


		public static readonly Column<Case, string> MySqlLoginColumn = new Column<Case, string>() { DefaultValue = "service" };
		[XmlAttribute]
		public string MySqlLogin
		{
			get { return MySqlLoginColumn.GetValue(this); }
			set { MySqlLoginColumn.SetValue(this, value); }
		}


		public static readonly Column<Case, string> MySqlPasswordColumn = new Column<Case, string>() { DefaultValue = "service" };
		[XmlAttribute]
		public string MySqlPassword
		{
			get { return MySqlPasswordColumn.GetValue(this); }
			set { MySqlPasswordColumn.SetValue(this, value); }
		}


		public List<CVS> CDRs
		{
			get;
			set;
		}

		public List<ViewFilter> Filters
		{
			get;
			set;
		}

		public Case()
		{
			CDRs = new List<CVS>();
			Filters = new List<ViewFilter>();
		}

	}


}
