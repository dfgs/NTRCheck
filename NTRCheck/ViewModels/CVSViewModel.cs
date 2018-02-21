using LogLib;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLib;

namespace NTRCheck.ViewModels
{
	public class CVSViewModel : ViewModel<CVS>
	{
		public int? CVSKEY
		{
			get { return Model.CVSKEY; }
			set { Model.CVSKEY=value;OnPropertyChanged(); }
		}

		public int? CVSUSR
		{
			get { return Model.CVSUSR; }
			set { Model.CVSUSR=value;OnPropertyChanged(); }
		}

		public int? CVSCHN
		{
			get { return Model.CVSCHN; }
			set { Model.CVSCHN=value;OnPropertyChanged(); }
		}

		public DateTime? UtcStartTime
		{
			get { return Model.UtcStartTime; }
			set { Model.UtcStartTime=value;OnPropertyChanged(); }
		}

		public DateTime? UtcEndTime
		{
			get { return Model.UtcEndTime; }
			set { Model.UtcEndTime=value;OnPropertyChanged(); }
		}

		public DateTime? CVSSDT
		{
			get { return Model.CVSSDT; }
			set { Model.CVSSDT=value;OnPropertyChanged(); }
		}

		public DateTime? CVSEDT
		{
			get { return Model.CVSEDT; }
			set { Model.CVSEDT=value;OnPropertyChanged(); }
		}

		public string CVSTYP
		{
			get { return Model.CVSTYP; }
			set { Model.CVSTYP=value;OnPropertyChanged(); }
		}

		public string CVSC05
		{
			get { return Model.CVSC05; }
			set { Model.CVSC05=value;OnPropertyChanged(); }
		}


		public CVSViewModel(ILogger Logger) : base(Logger)
		{
		}
	}
}
