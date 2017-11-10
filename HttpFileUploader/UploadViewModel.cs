using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using HttpTasks = HttpFileUploader.Tasks;

namespace HttpFileUploader
{
    public class FileItem : ViewModelBase
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value, "FilePath"); }
        }

        public string FileName
        {
            get { return Path.GetFileName(this.FilePath); }
        }

        private double progress;
        public double Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value, "Progress"); }
        }

        public FileItem(string filePath)
        {
            this.FilePath = filePath;
        }
    }

    class UploadViewModel : ViewModelBase
    {
        private string webHost = "127.0.0.1:8000";
        public string WebHost
        {
            get { return webHost; }
            set { SetProperty(ref webHost, value, "WebHost"); }
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value, "FilePath"); }
        }

        private string[] filePaths;

        /// <summary>
        /// Use for file item location
        /// </summary>
        IDictionary<string, FileItem> fileDict = new Dictionary<string, FileItem>();

        private ObservableCollection<FileItem> items = new ObservableCollection<FileItem>();
        public ObservableCollection<FileItem> Items
        {
            get { return items; }
        }

        public ICollectionView ItemsView
        {
            get { return CollectionViewSource.GetDefaultView(this.Items); }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) =>
                    {
                        OpenFileDialog openDlg = new OpenFileDialog();
                        openDlg.Multiselect = true;
                        if (openDlg.ShowDialog() == true)
                        {
                            this.FilePath = string.Join("\r\n", openDlg.FileNames);
                            this.filePaths = openDlg.FileNames;
                        }
                    }
                };
            }
        }

        public ICommand UploadCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) =>
                    {
                        HttpFactory.Instance.WebHost = this.WebHost;
                        FileChannel.Instance.Open();
                        foreach (var item in this.filePaths)
                        {
                            FileItem fi = new FileItem(item);
                            fileDict[item] = fi;
                            this.Items.Add(fi);
                        }


                        HttpTasks.TaskScheduler.Instance.AddFile(this.filePaths);
                    }
                };
            }
        }

        public void Load()
        {
            HttpTasks.TaskScheduler.Instance.Run();
            HttpTasks.TaskScheduler.Instance.OnUploading += (sender, e) =>
            {
                FileItem fileItem;
                if (this.fileDict.TryGetValue(e.FilePath, out fileItem))
                {
                    this.RunOnUIThreadAsync(() =>
                    {
                        fileItem.Progress = 100.0 * e.Progress / e.Len;
                    });
                }
            };
        }

        public void UnLoad()
        {
            HttpTasks.TaskScheduler.Instance.Stop();
        }

    }
}
