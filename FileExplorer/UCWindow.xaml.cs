using FileExplorer.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace FileExplorer
{


    /// <summary>
    /// Interaction logic for UCWindow.xaml
    /// </summary>
    public partial class UCWindow : Window
    {
        [Import]
        public CategoryGroupViewModel ViewModel { get; set; }

        public UCWindow()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                this.DataContext = ViewModel;
            };

        }
    }
}
