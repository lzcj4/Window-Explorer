using System.Windows;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCConfTextBox.xaml
    /// </summary>
    public partial class UCConfTextBox : UCConfBase
    {
        #region  DP       
        
        public string  Text
        {
            get { return (string )GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string ), typeof(UCConfTextBox), new PropertyMetadata(null));

        #endregion


        public UCConfTextBox()
        {
            InitializeComponent();
        }
        
    }
}
