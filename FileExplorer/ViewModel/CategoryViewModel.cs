using FileExplorer.Controls;
using FileExplorer.Helper;
using FileExplorer.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FileExplorer.ViewModel
{
    public class UCViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            FeatureModelViewModel item = value as FeatureModelViewModel;
            if (item.IsNull())
            {
                return null;
            }

            UCConfBase uc = null;
            if (item.Items.Count == 1)
            {
                uc = new UCConfTextBox();
            }
            else
            {
                uc = new UCConfCombox();
            }
            return uc;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryViewModel : DataViewModelBase<CategoryGroupViewModel, FeatureModelViewModel>
    {
        #region Props         

        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            set
            {
                SetProperty(ref isEditMode, value, "IsEditMode");
            }
        }

        #endregion

        public CategoryViewModel(CategoryGroupViewModel parent) : base(parent)
        {
        }

        public void LoadItems(string key, string title)
        {

        }

        public override void Add()
        {
            EditWindow win = new EditWindow();
            if (win.ShowDialog().Value)
            {
                AddNewItem(win);
            }
        }

        private void AddNewItem(EditWindow win)
        {
            IList<FeatureItem> items = win.Items;
            string title = win.FeatureTitle;

            if (items.IsNullOrEmpty())
            {
                return;
            }

            FeatureModelViewModel model = new FeatureModelViewModel(this);
            model.Title = title;
            items.All(item =>
            {
                model.Items.Add(item);
                return true;
            });
            this.Items.Add(model);
        }


        public override bool GetIsRemoveSelfEnabled()
        {
            return this.IsEditMode;
        }

        public override void Remove()
        {
            this.Parent.Remove(this);
        }

    }
}
