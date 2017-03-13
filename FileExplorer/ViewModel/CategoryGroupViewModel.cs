﻿
using FileExplorer.Helper;
using System.ComponentModel.Composition;

namespace FileExplorer.ViewModel
{
    [Export]
    public class CategoryGroupViewModel : DataViewModelBase<CategoryViewModel>
    {
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

    }
}
