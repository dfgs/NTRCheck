using NORMLib;
using NTRCheck.Models;
using NTRCheck.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ViewLib;
using ViewModelLib;
using ViewModelLib.PropertyViewModels;

namespace NTRCheck.Views
{
	/// <summary>
	/// Logique d'interaction pour FiltersView.xaml
	/// </summary>
	public partial class FiltersView : UserControl
	{
		public static readonly DependencyProperty ViewModelCollectionProperty = DependencyProperty.Register("ViewModelCollection", typeof(ViewFilterViewModelCollection), typeof(FiltersView));
		public ViewFilterViewModelCollection ViewModelCollection
		{
			get { return (ViewFilterViewModelCollection)GetValue(ViewModelCollectionProperty); }
			set { SetValue(ViewModelCollectionProperty, value); }
		}



		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(FiltersView));
		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}




		public FiltersView()
		{
			InitializeComponent();
		}

		private bool OnEditViewModel(IPropertyViewModelCollection Properties)
		{
			EditWindow editWindow;

			editWindow = new EditWindow() { Owner = Application.Current.MainWindow, PropertyViewModelCollection = Properties,DataContext=this };
			return editWindow.ShowDialog() ?? false;
		}

		private void AddCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = ViewModelCollection?.CanAdd() ?? false;
		}

		private async void AddCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				await ViewModelCollection.AddAsync(OnEditViewModel);
			}
			catch
			{
				ViewModelCollection.ErrorMessage = "Failed to add item";
			}
		}

		private void RemoveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = ViewModelCollection?.CanRemove() ?? false;
		}

		private async void RemoveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				await ViewModelCollection.RemoveAsync(null);
			}
			catch
			{
				ViewModelCollection.ErrorMessage = "Failed to remove item";
			}
		}

		private async void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (ViewModelCollection?.CanEdit() ?? false) await ViewModelCollection.EditAsync(OnEditViewModel);
			}
			catch
			{
				ViewModelCollection.ErrorMessage = "Failed to edit item";
			}
		}

		private void RefreshCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (ItemsSource!=null);
		}
		

		private bool OnFilter(object Item)
		{
			CVSViewModel cvs;

			cvs = Item as CVSViewModel;
			if (cvs == null) return false;

			foreach(ViewFilterViewModel filter in ViewModelCollection)
			{
				if (!filter.Filter(cvs.Model)) return false;
			}
			return true;
		}

		private void RefreshCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ICollectionView collectionView;

			try
			{
				collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
				collectionView.Filter = OnFilter;
				collectionView.Refresh();
			}
			catch
			{
				ViewModelCollection.ErrorMessage = "Failed to refresh";
			}
		}




	}
}
