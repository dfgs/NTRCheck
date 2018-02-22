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
	/// Logique d'interaction pour CVSView.xaml
	/// </summary>
	public partial class CVSView : UserControl
	{
		public static readonly DependencyProperty ViewModelCollectionProperty = DependencyProperty.Register("ViewModelCollection", typeof(IViewModelCollection), typeof(CVSView));
		public IViewModelCollection ViewModelCollection
		{
			get { return (IViewModelCollection)GetValue(ViewModelCollectionProperty); }
			set { SetValue(ViewModelCollectionProperty, value); }
		}

		public CVSView()
		{
			InitializeComponent();
		}

		



	}
}
