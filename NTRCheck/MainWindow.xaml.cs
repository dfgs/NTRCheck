using NORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NORMLib.SqlCE;
using ViewModelLib.PropertyViewModels;
using LogLib;
using NTRCheck.ViewModels;
using ViewLib;
using NTRCheck.Models;

namespace NTRCheck
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static string localDBFileName = "NTRCheck.mdf";

		private ILogger logger;

		private IServer localServer;
		private IConnectionFactory localConnectionFactory;
		private ICommandFactory localCommandFactory;

		private AppViewModel appViewModel;

		public MainWindow()
		{
			InitializeComponent();

			logger = new ConsoleLogger(new DefaultLogFormatter());

			localConnectionFactory = new SqlCEConnectionFactory(localDBFileName);
			localCommandFactory = new SqlCECommandFactory();
			localServer = new Server(localConnectionFactory, localCommandFactory);

			appViewModel = new AppViewModel(logger,localServer);
		}

		private void ShowError(Exception ex)
		{
			MessageBox.Show(this, ex.Message, "Error occured");
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			DBVersionController versionController;

			#region create local database
			if (!System.IO.File.Exists(localDBFileName))
			{
				try
				{
					System.IO.File.Create(localDBFileName).Close();
				}
				catch(Exception ex)
				{
					ShowError(ex);
					Close();
				}
			}
			
			versionController = new DBVersionController(localServer);
			try
			{
				versionController.Run();
			}
			catch(Exception ex)
			{
				ShowError(ex);
				Close();
			}
			#endregion

			try
			{
				await appViewModel.LoadAsync("");
			}
			catch(Exception ex)
			{
				ShowError(ex);
				Close();
			}
			DataContext = appViewModel;
		}


		private bool OnEditViewModel(IEnumerable<PropertyViewModel> Properties)
		{
			EditWindow editWindow;

			editWindow = new EditWindow() { Owner = this, DataContext = Properties };
			return editWindow.ShowDialog() ?? false;
		}
		private void NewCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;e.Handled = true;
			return;
		}

		
		private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				appViewModel.CurrentCase= await appViewModel.Cases.AddAsync(OnEditViewModel);
				await appViewModel.CVSs.UnloadAsync();
				await appViewModel.CVSs.LoadAsync(appViewModel.CurrentCase.CaseID.Value);
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true; e.Handled = true;
			return;
		}


		private async void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenCaseWindow window;

			try
			{
				window = new OpenCaseWindow();
				window.Owner = this;
				window.DataContext = appViewModel.Cases;
				if (!window.ShowDialog() ?? false) return;

				if (appViewModel.Cases.SelectedItem == null) return;
				appViewModel.CurrentCase = appViewModel.Cases.SelectedItem;
				await appViewModel.CVSs.UnloadAsync();
				await appViewModel.CVSs.LoadAsync(appViewModel.CurrentCase.CaseID.Value);
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}


		private void ImportCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = appViewModel?.CurrentCase!=null; e.Handled = true;
			return;
		}


		private async void ImportCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ImportCVSWindow window;

			try
			{
				window = new ImportCVSWindow();
				window.Owner = this;
				if (!window.ShowDialog() ?? false) return;

				await appViewModel.CVSs.LoadAsync(window.StartDate, window.EndDate);
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}





	}
}
