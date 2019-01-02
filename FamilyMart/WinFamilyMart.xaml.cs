using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for WinFamilyMart.xaml
    /// </summary>
    public partial class WinFamilyMart : Window
    {
        public WinFamilyMart()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
        }

       private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(gridFM, "全家套餐");
            }
        }
    }
}
