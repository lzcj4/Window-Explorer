using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source =
                new Uri("/FileExplorer;component/Theme/cn/Style.xaml", UriKind.RelativeOrAbsolute)
            });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source =
              new Uri("/FileExplorer;component/Style/RadioButtonStyle.xaml", UriKind.RelativeOrAbsolute)
            });
        }
    }
}
