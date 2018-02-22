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
using ViewModelLib;
using ViewModelLib.PropertyViewModels;

namespace NTRCheck.Views
{
	/// <summary>
	/// Logique d'interaction pour MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public static readonly DependencyProperty ViewModelCollectionProperty = DependencyProperty.Register("ViewModelCollection", typeof(IViewModelCollection), typeof(MainView));
		public IViewModelCollection ViewModelCollection
		{
			get { return (IViewModelCollection)GetValue(ViewModelCollectionProperty); }
			set { SetValue(ViewModelCollectionProperty, value); }
		}

		public MainView()
		{
			InitializeComponent();
		}

		private bool OnRemoveViewModel(IEnumerable<PropertyViewModel> Properties)
		{
			return MessageBox.Show(Application.Current.MainWindow, "Do you want to delete this item(s)", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		}


		private void RemoveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = ViewModelCollection?.CanRemove() ?? false;
		}

		private async void RemoveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				await ViewModelCollection.RemoveAsync(OnRemoveViewModel);
			}
			catch
			{
				ViewModelCollection.ErrorMessage = "Failed to remove item";
			}
		}


	}
}
