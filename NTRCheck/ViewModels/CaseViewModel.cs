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



		public static readonly DependencyProperty ServerProperty = DependencyProperty.Register("Server", typeof(IServer), typeof(CaseViewModel));
		public IServer Server
		{
			get { return (IServer)GetValue(ServerProperty); }
			set { SetValue(ServerProperty, value); }
		}


		public CaseViewModel(ILogger Logger) : base(Logger)
		{
		}

		protected override async Task OnLoadedAsync(Case Model)
		{
			IConnectionFactory connectionFactory;
			ICommandFactory commandFactory;

			connectionFactory = new MySqlConnectionFactory(CoreHostName, "recorder", MySqlLogin, MySqlPassword);
			commandFactory = new MySqlCommandFactory();

			Server = new Server(connectionFactory, commandFactory);

			await Task.Yield();
		}

	}



}
