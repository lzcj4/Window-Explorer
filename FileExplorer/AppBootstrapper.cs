using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.ServiceLocation;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace FileExplorer
{
    class AppBootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<ShellView>();
        }

        protected override void ConfigureAggregateCatalog()
        {
            //http://www.cnblogs.com/Clingingboy/archive/2009/01/13/prism_part5.html
            //DirectoryModuleCatalog
            //AssemblyCatalog
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
        }

        protected override CompositionContainer CreateContainer()
        {
            return new CompositionContainer(AggregateCatalog);
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();            
        }
    }
}
