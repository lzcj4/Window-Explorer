using System.Windows;

namespace HttpFileUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UploadViewModel ViewModel
        {
            get { return this.DataContext as UploadViewModel; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new UploadViewModel();
            this.Loaded += (sender, e) => { this.ViewModel.Load(); };
            this.Closing += (sender, e) => { this.ViewModel.UnLoad(); };
        }
    }
}
