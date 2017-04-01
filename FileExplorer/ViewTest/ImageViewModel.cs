using FileExplorer.Helper;
using FileExplorer.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.ViewTest
{
    [Export]
    public class ImageViewModel : ViewModelBase, IDropHandler
    {
        private ObservableCollection<ImageItem> items = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> Items
        {
            get { return items; }
            private set { SetProperty(ref items, value, "Items"); }
        }

        private ObservableCollection<ImageItem> editingItems = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> EditingItems
        {
            get { return editingItems; }
            private set { SetProperty(ref editingItems, value, "EditingItems"); }
        }

        public void LoadImages()
        {
            Items.Clear();
            for (int i = 1; i < 9; i++)
            {
                Items.Add(new ImageItem() { FilePath = string.Format(@" E:\素材\Imag\{0}.png", i) });
            }

            this.EditingItems.Clear();
            for (int i = 0; i < 4; i++)
            {
                this.EditingItems.Add(Items[i]);
            }

            DragDrop.DefaultDropHandler = this;
        }

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            /// TOTO: 需要加入去重
            if (dropInfo.Data is ImageItem || dropInfo.Data is IEnumerable<ImageItem>)
            {
                if (this.EditingItems.Any(item => item == (dropInfo.Data as ImageItem)))
                {
                    dropInfo.Effects = DragDropEffects.None;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                }
            }
        }

        public void OnDrop(DropInfo dropInfo)
        {
            if (dropInfo.IsNull() || dropInfo.TargetItem.IsNull() ||
                !(dropInfo.TargetItem is FrameworkElement))
            {
                return;
            }

            ImageItem imgItem = dropInfo.Data as ImageItem;
            IEnumerable<ImageItem> dragItems = dropInfo.Data as IEnumerable<ImageItem>;

            UCImageView imgView = (dropInfo.TargetItem as FrameworkElement).TryFindParent<UCImageView>();
            if (!imgView.IsNull())
            {
                if (!imgItem.IsNull())
                {
                    this.EditingItems.Remove(imgView.ViewModel);
                    imgView.DataContext = imgItem;
                    this.EditingItems.Add(imgItem);
                }
                else if (!dragItems.IsNullOrEmpty())
                {
                    ImageItem firstItem = items.FirstOrDefault();
                    this.EditingItems.Remove(imgView.ViewModel);
                    imgView.DataContext = firstItem;
                    this.EditingItems.Add(firstItem);
                }
            }
            else
            {
                Canvas parent = dropInfo.TargetItem as Canvas;
                if (parent.IsNull())
                {
                    parent = (dropInfo.TargetItem as FrameworkElement).TryFindParent<Canvas>();
                }

                if (!parent.IsNull())
                {
                    if (!imgItem.IsNull())
                    {
                        this.EditingItems.Add(imgItem);
                    }
                    else if (!dragItems.IsNullOrEmpty())
                    {
                        dragItems.All(i => { this.EditingItems.Add(i); return true; });
                    }
                }
            }
        }

        #endregion
    }
}
