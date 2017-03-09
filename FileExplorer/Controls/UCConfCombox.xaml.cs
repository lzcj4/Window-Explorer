
using FileExplorer.Helper;
using FileExplorer.Model;
using FileExplorer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCConfCombox.xaml
    /// </summary>
    public partial class UCConfCombox : UCConfBase
    {
        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(UCConfCombox), new PropertyMetadata(false));

        public UCConfCombox()
        {
            InitializeComponent();
        }

        private void imgItemDel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img.IsNull())
            {
                return;
            }
            FeatureItem item = img.DataContext as FeatureItem;
            (this.DataContext as FeatureModelViewModel).Remove(item);
        }
    }
}
