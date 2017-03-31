
using FileExplorer.Controls;
using FileExplorer.Helper;
using FileExplorer.ViewTest;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace FileExplorer.ViewModel
{
    [Export]
    public class CategoryGroupViewModel : DataViewModelBase<CategoryViewModel>
    {
        [Import]
        IRegionManager regionManager;

        CategoryViewModel lastItem = null;      

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
            regionManager.AddToRegion(ViewRegions.Region_Top, new UCView());
            // regionManager.AddToRegion(ViewRegions.Region_Top, new UCConfCategoryGroup());
        }
    }
}
