using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace FileExplorer.ViewTest
{
    [Export(typeof(ImageViewModel))]
    public class ImageViewModel : ViewModelBase
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
        }
    }
}
