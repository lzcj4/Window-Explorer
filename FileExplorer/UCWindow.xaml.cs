using FileExplorer.ViewModel;
using System.Windows;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for UCWindow.xaml
    /// </summary>
    public partial class UCWindow : Window
    {
        public UCWindow()
        {
            InitializeComponent();

            this.DataContext = new CategoryGroupViewModel();
        }
    }
}
