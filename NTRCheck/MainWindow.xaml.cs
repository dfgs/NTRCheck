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
using Microsoft.Win32;

namespace NTRCheck
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ILogger logger;

		public static readonly DependencyProperty CaseViewModelProperty = DependencyProperty.Register("CaseViewModel", typeof(CaseViewModel), typeof(MainWindow));
		public CaseViewModel CaseViewModel
		{
			get { return (CaseViewModel)GetValue(CaseViewModelProperty); }
			private set { SetValue(CaseViewModelProperty, value); }
		}


		public MainWindow()
		{
			InitializeComponent();

			logger = new ConsoleLogger(new DefaultLogFormatter());
					
		}

		private void ShowError(Exception ex)
		{
			MessageBox.Show(this, ex.Message, "Error occured");
		}

		
		private bool OnEditViewModel(IEnumerable<PropertyViewModel> Properties)
		{
			EditWindow editWindow;

			editWindow = new EditWindow() { Owner = this, DataContext = Properties };
			return editWindow.ShowDialog() ?? false;
		}
		private void NewCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel==null) || (CaseViewModel.CDRs.Status == Statuses.Idle); 
		}
				
		private async void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CaseViewModel newCase;
			PropertyViewModelCollection<CaseViewModel> properties;
			EditWindow editWindow;


			try
			{
				newCase = new CaseViewModel(logger);
				await newCase.LoadAsync(new Case());

				properties = new PropertyViewModelCollection<CaseViewModel>(logger,null);
				await properties.LoadAsync();
				await properties.ReadComponentsAsync(newCase);
				editWindow = new EditWindow() { Owner = this, PropertyViewModelCollection = properties };

				if (editWindow.ShowDialog() ?? false)
				{
					await properties.WriteComponentsAsync(newCase);
					this.CaseViewModel = newCase;
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return;
			}
		}

		private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel == null) || (CaseViewModel.CDRs.Status == Statuses.Idle); 
			return;
		}


		private async void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog window;
			CaseViewModel newCaseViewModel;

			try
			{
				window = new OpenFileDialog();
				window.DefaultExt = "xml";
				window.FileName = "*.xml";
				window.Filter = "xml files|*.xml|All files|*.*";
				if (!window.ShowDialog() ?? false) return;

				newCaseViewModel = new CaseViewModel(logger);
				this.CaseViewModel = newCaseViewModel;

				await newCaseViewModel.LoadAsync(window.FileName);

				
			}
			catch(Exception ex)
			{
				ShowError(ex);
			}
		}
		private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel != null) && (CaseViewModel.CDRs.Status == Statuses.Idle);
			return;
		}


		private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog window;

			try
			{
				if (CaseViewModel.FileName == null)
				{
					window = new SaveFileDialog();
					window.DefaultExt = "xml";
					window.FileName="new case";
					window.Filter = "xml files|*.xml|All files|*.*";
					if (!window.ShowDialog() ?? false) return;

					CaseViewModel.FileName = window.FileName;
				}
							
				CaseViewModel.Save();
			}
			catch
			{
				CaseViewModel.ErrorMessage = "Failed to save";
			}
		}
		private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel != null) && (CaseViewModel.CDRs.Status == Statuses.Running);
		}


		private async void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await CaseViewModel.CDRs.StopAsync();
		}

		private void ImportCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel !=null) && (CaseViewModel.CDRs.Status == Statuses.Idle) && (CaseViewModel.CDRs.IsLoading == false); 
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

				elapsed=await CaseViewModel.CDRs.ImportAsync(CaseViewModel.CreateServer(), window.StartDate, window.EndDate);
				MessageBox.Show(this, $"CVS imported in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				CaseViewModel.CDRs.ErrorMessage = "Failed to import CVS";
				return;
			}
		}
		private void ClearCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (CaseViewModel!=null) && (CaseViewModel.CDRs.CanClear());
		}

		private bool ClearCallBack()
		{
			return MessageBox.Show(this, "Do you want to clear all item(s)", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}
		private async void ClearCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{

			try
			{
				await CaseViewModel.CDRs.ClearAsync(ClearCallBack);
			}
			catch
			{
				CaseViewModel.CDRs.ErrorMessage = "Failed to clear CVS";
				return;
			}
		}

		private void AssociateCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = CaseViewModel?.CDRs?.CanClear() ?? false;
		}

		
		private async void AssociateCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			TimeSpan elapsed;

			try
			{
				elapsed=await CaseViewModel.CDRs.AssociateAsync();
				MessageBox.Show(this, $"CVS associated in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				CaseViewModel.CDRs.ErrorMessage = "Failed to associate CVS";
				return;
			}
		}


		private void AnalyseCVSCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = CaseViewModel?.CDRs?.CanClear() ?? false;
		}


		private async void AnalyseCVSCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			TimeSpan elapsed;

			try
			{
				elapsed = await CaseViewModel.CDRs.AnalyseAsync();
				MessageBox.Show(this, $"CVS analysed in {elapsed}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch
			{
				CaseViewModel.CDRs.ErrorMessage = "Failed to analyse CVS";
				return;
			}
		}




	}
}
