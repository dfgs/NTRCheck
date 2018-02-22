using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORMLib;
using NORMLib.VersionControl;
using NTRCheck.Models;

namespace NTRCheck
{
	public class DBVersionController : VersionController
	{
		public DBVersionController(IServer Server) : base(Server, "NTRCheck")
		{
		}

		[Revision(0)]
		public static IEnumerable<IQuery> UpgradeToRevision0()
		{
			yield return new CreateTable<Case>(Case.CaseIDColumn, Case.DescriptionColumn, Case.CoreHostNameColumn, Case.MySqlLoginColumn, Case.MySqlPasswordColumn);
			yield return new CreateTable<CVS>(CVS.CVSIDColumn, CVS.CaseIDColumn, CVS.ParentCVSIDColumn, CVS.CVSKEYColumn, CVS.CVSUSRColumn, CVS.CVSCHNColumn, CVS.UtcStartTimeColumn, CVS.UtcEndTimeColumn, CVS.CVSSDTColumn, CVS.CVSEDTColumn, CVS.CVSTYPColumn, CVS.CVSC05Column);

		}
	
		[Revision(1)]
		public static IEnumerable<IQuery> UpgradeToRevision1()
		{
			yield return new CreateRelation<Case, CVS, int?>(Case.CaseIDColumn, CVS.CaseIDColumn);
			yield return new CreateRelation<CVS, CVS, int?>(CVS.CVSIDColumn, CVS.ParentCVSIDColumn);

		}


	}

}
