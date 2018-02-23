using LogLib;
using NORMLib;
using NORMLib.MySql;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;
using ViewModelLib.Attributes;

namespace NTRCheck.ViewModels
{
	public class CaseViewModel : ViewModel<Case>
	{
		
		public int? CaseID
		{
			get { return Model.CaseID; }
			set { Model.CaseID = value;OnPropertyChanged(); }
		}

		[Editable(IsMandatory = true)]
		public string Description
		{
			get { return Model.Description; }
			set { Model.Description = value;OnPropertyChanged(); }
		}

		[Editable(Header = "Core hostname", IsMandatory = true)]
		public string CoreHostName
		{
			get { return Model.CoreHostName; }
			set { Model.CoreHostName = value; OnPropertyChanged(); }
		}

		[Editable(Header = "MySql login", IsMandatory = true)]
		public string MySqlLogin
		{
			get { return Model.MySqlLogin; }
			set { Model.MySqlLogin = value; OnPropertyChanged(); }
		}

		[Editable(Header = "MySql password", IsMandatory = true)]
		public string MySqlPassword
		{
			get { return Model.MySqlPassword; }
			set { Model.MySqlPassword = value; OnPropertyChanged(); }
		}


		public CDRViewModelCollection CDRs
		{
			get;
			private set;
		}



		public CaseViewModel(ILogger Logger) : base(Logger)
		{
			CDRs = new CDRViewModelCollection(Logger);
		}

		protected override async Task OnLoadedAsync(Case Model)
		{
			await base.OnLoadedAsync(Model);
			await CDRs.LoadAsync(this);
		}
		public IServer CreateServer()
		{
			IConnectionFactory connectionFactory;
			ICommandFactory commandFactory;

			connectionFactory = new MySqlConnectionFactory(Model.CoreHostName, "recorder", Model.MySqlLogin, Model.MySqlPassword);
			commandFactory = new MySqlCommandFactory();

			return new Server(connectionFactory, commandFactory);
		}
		

	}



}
