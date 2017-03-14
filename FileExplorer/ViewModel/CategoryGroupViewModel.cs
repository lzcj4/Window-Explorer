
using FileExplorer.Controls;
using FileExplorer.Helper;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace FileExplorer.ViewModel
{
    [Export]
    public class CategoryGroupViewModel : DataViewModelBase<CategoryViewModel>
    {
        CategoryViewModel lastItem = null;
        [Import]
        IRegionManager rm;

        public override void Add()
        {
            if (!lastItem.IsNull())
            {
                lastItem.IsEditMode = false;

            }
            lastItem = new CategoryViewModel(this) { IsEditMode = true };
            this.Items.Add(lastItem);
        }


        public void LoadViews()
        {
            rm.Regions["up"].Add(new UCConfCategoryGroup());
        }

    }
}
