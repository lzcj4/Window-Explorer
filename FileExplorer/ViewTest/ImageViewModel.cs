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
    [Export(typeof(ImageViewModel))]
    public class ImageViewModel : ViewModelBase, IDropHandler
    {
        private ObservableCollection<ImageItem> items = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> Items
        {
            get { return items; }
            private set { SetProperty(ref items, value, "Items"); }
        }

        private ObservableCollection<ImageItem> editintItems = new ObservableCollection<ImageItem>();
        public ObservableCollection<ImageItem> EditintItems
        {
            get { return editintItems; }
            private set { SetProperty(ref editintItems, value, "EditintItems"); }
        }

        public void LoadImages()
        {
            Items.Clear();
            Items.Add(new ImageItem() { FilePath = @"E:\aa.png" });
            Items.Add(new ImageItem() { FilePath = @"E:\bb.png" });
            Items.Add(new ImageItem() { FilePath = @"E:\aa.png" });

            this.EditintItems.Clear();
            Items.All(item => { this.EditintItems.Add(item); return true; });

            DragDrop.DefaultDropHandler = this;
        }

        #region IDropHandler

        public void OnDragOver(DropInfo dropInfo)
        {
            /// TOTO: 需要加入去重
            if (dropInfo.Data is ImageItem || dropInfo.Data is IEnumerable<ImageItem>)
            {
                if (this.EditintItems.Any(item => item == (dropInfo.Data as ImageItem)))
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
                    this.EditintItems.Remove(imgView.ViewModel);
                    imgView.DataContext = imgItem;
                }
                else if (!dragItems.IsNullOrEmpty())
                {
                    this.EditintItems.Remove(imgView.ViewModel);
                    imgView.DataContext = items.FirstOrDefault();
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
                        this.EditintItems.Add(imgItem);
                    }
                    else if (!dragItems.IsNullOrEmpty())
                    {
                        dragItems.All(i => { this.EditintItems.Add(i); return true; });
                    }
                }
            }
        }

        #endregion
    }
}
