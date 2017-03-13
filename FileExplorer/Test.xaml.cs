using FileExplorer.Helper;
using FileExplorer.ViewModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            this.DataContext = new TestViewModel();
            this.Loaded += (sender, e) =>
            {
                var i = UIAttachedProp.GetCheckedIcon(this.radioButton);
                var i2 = UIAttachedProp.GetUncheckedIcon(this.radioButton);
                string sbName = "RotateBorderStoryboard";
                bool isContained = this.Resources.Contains(sbName);
                var sb = this.Resources[sbName] as Storyboard;


                //Storyboard.SetTargetName(this.border, "border");
                //sb.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Begin();

            };
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindows = new MainWindow();
            mainWindows.Show();
        }

     
    }
}
