using FileExplorer.ViewModel;
using FileExplorer.Helper;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System;
using System.ComponentModel.Composition;

namespace FileExplorer.ViewTest
{
    public enum ImageSize
    {
        Normal = 0,
        Min = 1,
        Max = 2
    }

    [Export]
    public class ImageItem : ViewModelBase, IDragSource
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value, "FilePath", "FileName"); }
        }

        public string FileName
        {
            get
            {
                return this.FilePath.IsNullOrEmpty() ? string.Empty : Path.GetFileName(this.FilePath);
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get
            {
                if (imageSource.IsNull() && !this.FilePath.IsNullOrEmpty() &&
                    File.Exists(this.filePath))
                {
                    imageSource = new BitmapImage(new Uri(this.FilePath, UriKind.RelativeOrAbsolute));
                }
                return imageSource;
            }

            set { SetProperty(ref imageSource, value, "ImageSource"); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value, "IsSelected"); }
        }

        private ImageSize sizeMode = ImageSize.Normal;
        public ImageSize SizeMode
        {
            get { return sizeMode; }
            set { SetProperty(ref sizeMode, value, "SizeMode", "IsMin", "IsMax"); }
        }

        public bool IsMin { get { return this.SizeMode == ImageSize.Min; } }
        public bool IsMax { get { return this.SizeMode == ImageSize.Max; } }


        #region IDragSource

        public void StartDrag(DragInfo dragInfo)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
