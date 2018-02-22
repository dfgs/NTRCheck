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
			e.Handled = true; e.CanExecute = (appViewModel?.CVSs==null) || (appViewModel?.CVSs?.Status == Statuses.Idle); 
		}

		
		private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CaseViewModel Case;
			try
			{
				Case= await appViewModel.Cases.AddAsync(OnEditViewModel);
				appViewModel.CVSs = new CVSViewModelCollection(logger, localServer, Case);
				await appViewModel.CVSs.LoadAsync();
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CVSs == null) || (appViewModel?.CVSs?.Status == Statuses.Idle); 
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
				
				appViewModel.CVSs = new CVSViewModelCollection(logger, localServer, appViewModel.Cases.SelectedItem);
				await appViewModel.CVSs.LoadAsync();

			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CVSs != null) && (appViewModel?.CVSs?.Status == Statuses.Running);
		}


		private async void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await appViewModel.CVSs.StopAsync();
		}

		private void ImportCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CVSs!=null) && (appViewModel?.CVSs?.Status == Statuses.Idle) && (appViewModel?.CVSs?.IsLoading == false); 
		}


		private async void ImportCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ImportCVSWindow window;
			TimeSpan elapsed;

			try
			{
				window = new ImportCVSWindow();
				window.Owner = this;
				window.StartDate = new DateTime(2015, 10, 13);
				window.EndDate = new DateTime(2015, 10, 13);
				if (!window.ShowDialog() ?? false) return;

				elapsed=await appViewModel.CVSs.ImportAsync(window.StartDate, window.EndDate);
				MessageBox.Show(this, $"CVS imported in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				appViewModel.CVSs.ErrorMessage = "Failed to import CVS";
				return;
			}
		}
		private void ClearCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = appViewModel?.CVSs?.CanClear()??false;
		}

		private bool ClearCallBack()
		{
			return MessageBox.Show(this, "Do you want to clear all item(s)", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}
		private async void ClearCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{

			try
			{
				await appViewModel.CVSs.ClearAsync(ClearCallBack);
			}
			catch
			{
				appViewModel.CVSs.ErrorMessage = "Failed to clear CVS";
				return;
			}
		}




	}
}
