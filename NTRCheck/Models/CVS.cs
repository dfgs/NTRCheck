using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NTRCheck.Models
{
	[Table("cvs")]
	public class CVS
	{
		public static readonly Column<CVS, int?> CVSKEYColumn = new Column<CVS, int?>() { IsPrimaryKey=true} ;
		[XmlElement(IsNullable = true)]
		public int? CVSKEY
		{
			get { return CVSKEYColumn.GetValue(this); }
			set { CVSKEYColumn.SetValue(this, value); }
		}
		
		public static readonly Column<CVS, int?> CVSUSRColumn = new Column<CVS, int?>();
		[XmlElement(IsNullable = true)]
		public int? CVSUSR
		{
			get { return CVSUSRColumn.GetValue(this); }
			set { CVSUSRColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, int?> CVSCHNColumn = new Column<CVS, int?>();
		[XmlElement(IsNullable = true)]
		public int? CVSCHN
		{
			get { return CVSCHNColumn.GetValue(this); }
			set { CVSCHNColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, DateTime?> UtcStartTimeColumn = new Column<CVS, DateTime?>();
		[XmlElement(IsNullable = true)]
		public DateTime? UtcStartTime
		{
			get { return UtcStartTimeColumn.GetValue(this); }
			set { UtcStartTimeColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> UtcEndTimeColumn = new Column<CVS, DateTime?>();
		[XmlElement(IsNullable = true)]
		public DateTime? UtcEndTime
		{
			get { return UtcEndTimeColumn.GetValue(this); }
			set { UtcEndTimeColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> CVSSDTColumn = new Column<CVS, DateTime?>();
		[XmlElement(IsNullable = true)]
		public DateTime? CVSSDT
		{
			get { return CVSSDTColumn.GetValue(this); }
			set { CVSSDTColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> CVSEDTColumn = new Column<CVS, DateTime?>();
		[XmlElement(IsNullable = true)]
		public DateTime? CVSEDT
		{
			get { return CVSEDTColumn.GetValue(this); }
			set { CVSEDTColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, string> CVSTYPColumn = new Column<CVS, string>();
		[XmlElement(IsNullable = true)]
		public string CVSTYP
		{
			get { return CVSTYPColumn.GetValue(this); }
			set { CVSTYPColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, string> CVSC05Column = new Column<CVS, string>();
		[XmlElement(IsNullable = true)]
		public string CVSC05
		{
			get { return CVSC05Column.GetValue(this); }
			set { CVSC05Column.SetValue(this, value); }
		}


		public CVSStatuses Status
		{
			get;
			set;
		}


		public List<CVS> VOXs
		{
			get;
			set;
		}

		public CVS()
		{
			VOXs = new List<CVS>();
		}

	}

}
