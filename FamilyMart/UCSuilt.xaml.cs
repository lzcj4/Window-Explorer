using System.Windows;
using System.Windows.Controls;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for UCSuilt.xaml
    /// </summary>
    public partial class UCSuilt : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UCSuilt), new PropertyMetadata("Fri.套餐A"));


        public UCSuilt()
        {
            InitializeComponent();
        }
    }
}
