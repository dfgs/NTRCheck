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

		private ILogger logger;
		
		public MainWindow()
		{
			InitializeComponent();

			logger = new ConsoleLogger(new DefaultLogFormatter());
					
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
			e.Handled = true; e.CanExecute = (appViewModel?.CDRs==null) || (appViewModel?.CDRs?.Status == Statuses.Idle); 
		}

		
		private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				appViewModel.CurrentCase= await appViewModel.Cases.AddAsync(OnEditViewModel);
				appViewModel.CDRs = new CDRViewModelCollection(logger, localServer);
				await appViewModel.CDRs.LoadAsync(appViewModel.CurrentCase);
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CDRs == null) || (appViewModel?.CDRs?.Status == Statuses.Idle); 
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

				appViewModel.CDRs = new CDRViewModelCollection(logger, localServer);
				await appViewModel.CDRs.LoadAsync(appViewModel.CurrentCase);

			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CDRs != null) && (appViewModel?.CDRs?.Status == Statuses.Running);
		}


		private async void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await appViewModel.CDRs.StopAsync();
		}

		private void ImportCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (appViewModel?.CDRs!=null) && (appViewModel?.CDRs?.Status == Statuses.Idle) && (appViewModel?.CDRs?.IsLoading == false); 
		}


		private async void ImportCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ImportCVSWindow window;
			TimeSpan elapsed;

			try
			{
				window = new ImportCVSWindow();
				window.Owner = this;
				window.StartDate = new DateTime(2018, 01, 17);
				window.EndDate = new DateTime(2018, 01, 17);
				if (!window.ShowDialog() ?? false) return;

				elapsed=await appViewModel.CDRs.ImportAsync(appViewModel.CurrentCase, window.StartDate, window.EndDate);
				MessageBox.Show(this, $"CVS imported in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				appViewModel.CDRs.ErrorMessage = "Failed to import CVS";
				return;
			}
		}
		private void ClearCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = appViewModel?.CDRs?.CanClear()??false;
		}

		private bool ClearCallBack()
		{
			return MessageBox.Show(this, "Do you want to clear all item(s)", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}
		private async void ClearCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{

			try
			{
				await appViewModel.CDRs.ClearAsync(appViewModel.CurrentCase, ClearCallBack);
			}
			catch
			{
				appViewModel.CDRs.ErrorMessage = "Failed to clear CVS";
				return;
			}
		}

		private void AssociateCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = appViewModel?.CDRs?.CanClear() ?? false;
		}

		
		private async void AssociateCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			TimeSpan elapsed;

			try
			{
				elapsed=await appViewModel.CDRs.AssociateAsync();
				MessageBox.Show(this, $"CVS associated in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				appViewModel.CDRs.ErrorMessage = "Failed to associate CVS";
				return;
			}
		}




	}
}
