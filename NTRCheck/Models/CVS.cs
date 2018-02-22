using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTRCheck.Models
{
	[Table("cvs")]
	public class CVS
	{
		public static readonly Column<CVS, int?> CVSIDColumn = new Column<CVS, int?>() { IsPrimaryKey = true, IsIdentity = true };
		public int? CVSID
		{
			get { return CVSIDColumn.GetValue(this); }
			set { CVSIDColumn.SetValue(this, value); }
		}

		public static readonly Column<CVS, int?> CaseIDColumn = new Column<CVS, int?>();
		public int? CaseID
		{
			get { return CaseIDColumn.GetValue(this); }
			set { CaseIDColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, int?> ParentCVSIDColumn = new Column<CVS, int?>() { IsNullable=true }; 
		public int? ParentCVSID
		{
			get { return ParentCVSIDColumn.GetValue(this); }
			set { ParentCVSIDColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, int?> CVSKEYColumn = new Column<CVS, int?>();
		public int? CVSKEY
		{
			get { return CVSKEYColumn.GetValue(this); }
			set { CVSKEYColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, int?> CVSUSRColumn = new Column<CVS, int?>();
		public int? CVSUSR
		{
			get { return CVSUSRColumn.GetValue(this); }
			set { CVSUSRColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, int?> CVSCHNColumn = new Column<CVS, int?>();
		public int? CVSCHN
		{
			get { return CVSCHNColumn.GetValue(this); }
			set { CVSCHNColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, DateTime?> UtcStartTimeColumn = new Column<CVS, DateTime?>();
		public DateTime? UtcStartTime
		{
			get { return UtcStartTimeColumn.GetValue(this); }
			set { UtcStartTimeColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> UtcEndTimeColumn = new Column<CVS, DateTime?>();
		public DateTime? UtcEndTime
		{
			get { return UtcEndTimeColumn.GetValue(this); }
			set { UtcEndTimeColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> CVSSDTColumn = new Column<CVS, DateTime?>();
		public DateTime? CVSSDT
		{
			get { return CVSSDTColumn.GetValue(this); }
			set { CVSSDTColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, DateTime?> CVSEDTColumn = new Column<CVS, DateTime?>();
		public DateTime? CVSEDT
		{
			get { return CVSEDTColumn.GetValue(this); }
			set { CVSEDTColumn.SetValue(this, value); }
		}



		public static readonly Column<CVS, string> CVSTYPColumn = new Column<CVS, string>();
		public string CVSTYP
		{
			get { return CVSTYPColumn.GetValue(this); }
			set { CVSTYPColumn.SetValue(this, value); }
		}


		public static readonly Column<CVS, string> CVSC05Column = new Column<CVS, string>();
		public string CVSC05
		{
			get { return CVSC05Column.GetValue(this); }
			set { CVSC05Column.SetValue(this, value); }
		}



	}

}
