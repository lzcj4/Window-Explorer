using FileExplorer.Helper;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.ViewTest
{
    /// <summary>
    /// Interaction logic for UCView.xaml
    /// </summary>
    public partial class UCView : UserControl
    {
        [Import(typeof(ImageViewModel))]
        //public Lazy<ImageViewModel> ViewModel { get; set; }
        public ImageViewModel ViewModel { get; set; }
        public UCView()
        {
            InitializeComponent();
            ViewModel = new ImageViewModel();
            this.DataContext = ViewModel;
            this.ViewModel.LoadImages();

            this.listImages.MouseDoubleClick += ListImages_MouseDoubleClick;

        }

        private void ListImages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ImageItem imageItem = listImages.SelectedItem as ImageItem;
            if (imageItem.IsNull())
            {
                return;
            }
            if (imageItem.IsMin)
            {
                ucView.Resume(imageItem);
            }
            else
            {
                ucView.AddItem(imageItem);
            }
        }

        private void radioTile_Checked(object sender, RoutedEventArgs e)
        {
            ucView.LayoutMode = LayoutMode.Tile;
        }

        private void radioCascade_Checked(object sender, RoutedEventArgs e)
        {
            ucView.LayoutMode = LayoutMode.Cascade;
        }
    }
}
