using System.Windows.Controls;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for UCPage.xaml
    /// </summary>
    public partial class UCPage : UserControl
    {
        public Grid GridRoot { get { return this.gridRoot; } }
        public UCPage()
        {
            InitializeComponent();
        }
    }
}
