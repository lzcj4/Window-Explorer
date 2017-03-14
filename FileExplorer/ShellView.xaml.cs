using FileExplorer.Helper;
using FileExplorer.ViewModel;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileExplorer
{

    [Export]
    /// <summary>
    /// Interaction logic for UCWindow.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        [Import]
        public Lazy<CategoryGroupViewModel> ViewModel { get; set; }
     
        public ShellView()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                var fp = SysSettings.GetInstance().Quick_FilePath;
                var sk = SysSettings.GetInstance().Quick_Save_Keys;
                SysSettings.GetInstance().Quick_FilePath = @"D:\";
                SysSettings.GetInstance().Quick_Save_Keys = new Tuple<ModifierKeys, Keys>(ModifierKeys.Control | ModifierKeys.Alt, Keys.B);
                SysSettings.GetInstance().Save();
                this.DataContext = ViewModel.Value;
                ViewModel.Value.LoadViews();
            };

        }
    }
}
