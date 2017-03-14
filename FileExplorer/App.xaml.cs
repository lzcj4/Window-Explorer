using FileExplorer.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using System.Collections.Generic;

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
            
            AppBootstrapper bootstrapper = new AppBootstrapper();
            bootstrapper.Run();
        }

    }
}
