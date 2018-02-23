using LogLib;
using NORMLib;
using NORMLib.MySql;
using NTRCheck.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using ViewModelLib;
using ViewModelLib.Attributes;

namespace NTRCheck.ViewModels
{
	public class CaseViewModel : ViewModel<Case>
	{

		public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(CaseViewModel));
		public string FileName
		{
			get { return (string)GetValue(FileNameProperty); }
			set { SetValue(FileNameProperty, value); }
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


		public CVSViewModelCollection CDRs
		{
			get;
			private set;
		}



		public CaseViewModel(ILogger Logger) : base(Logger)
		{
			CDRs = new CVSViewModelCollection(Logger);
		}

		protected override async Task OnLoadedAsync(Case Model)
		{
			await base.OnLoadedAsync(Model);
			await CDRs.LoadAsync(Model.CDRs);
		}


		public async Task LoadAsync(string FileName)
		{
			XmlSerializer serializer;
			Case model;

			this.FileName = FileName;
			try
			{
				using (FileStream stream = new FileStream(FileName, FileMode.Open))
				{
					serializer = new XmlSerializer(typeof(Case));
					model=(Case)serializer.Deserialize(stream);
					stream.Flush();
				}
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
			}
			await LoadAsync(model);
		}

		public IServer CreateServer()
		{
			IConnectionFactory connectionFactory;
			ICommandFactory commandFactory;

			connectionFactory = new MySqlConnectionFactory(Model.CoreHostName, "recorder", Model.MySqlLogin, Model.MySqlPassword);
			commandFactory = new MySqlCommandFactory();

			return new Server(connectionFactory, commandFactory);
		}



		public void Save()
		{
			XmlSerializer serializer;

			try
			{
				using (FileStream stream = new FileStream(FileName,FileMode.Create))
				{
					serializer = new XmlSerializer(typeof(Case));
					serializer.Serialize(stream, Model);
					stream.Flush();
				}
			}
			catch (Exception ex)
			{
				Log(ex);
				throw (ex);
			}
			finally
			{
			}

		}



	}



}
