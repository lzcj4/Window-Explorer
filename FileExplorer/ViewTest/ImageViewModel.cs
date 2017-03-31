using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using FileExplorer.Helper;
using System.Windows;

namespace FileExplorer.ViewTest
{
    [Export(typeof(ImageViewModel))]
    public class ImageViewModel : ViewModelBase, IDropHandler
    {
        private ObservableCollection<ImageItem> items = new ObservableCollection<ImageItem>();

        public ObservableCollection<ImageItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value, "Items"); }
        }

        public void LoadImages()
        {
            Items.Clear();
            Items.Add(new ImageItem() { FilePath = @"E:\aa.png" });
            Items.Add(new ImageItem() { FilePath = @"E:\bb.png" });
            Items.Add(new ImageItem() { FilePath = @"E:\aa.png" });

            DragDrop.DefaultDropHandler = this;
        }


        #region IDropHandler
        public void OnDragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is ImageItem || dropInfo.Data is IEnumerable<ImageItem>)
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public void OnDrop(DropInfo dropInfo)
        {
            UCImageView imgView = dropInfo.TargetItem as UCImageView;
            ImageItem imgItem = dropInfo.Data as ImageItem;
            IEnumerable<ImageItem> items = dropInfo.Data as IEnumerable<ImageItem> ;
            if (!imgView.IsNull() )
            {
                if(!imgItem.IsNull())
                {
                    imgView.DataContext = imgItem;
                }
                else if (!items.IsNullOrEmpty())
                {
                    imgView.DataContext = items.FirstOrDefault();
                }
            }
        }

        #endregion
    }
}
