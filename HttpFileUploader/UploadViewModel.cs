using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HttpFileUploader
{
    class UploadViewModel : ViewModelBase
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value, "FilePath"); }
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
                        openDlg.Multiselect = false;
                        if (openDlg.ShowDialog() == true)
                        {
                            this.FilePath = openDlg.FileName;
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
                        if (File.Exists(this.FilePath))
                        {
                            //MyHttp http = new MyHttp();
                            //http.Upload("http://127.0.0.1:8000/file/upload/", this.FilePath);

                            UploadManager uploader = new UploadManager();
                            //uploader.Upload("http://172.16.4.166:8000/file/upload/", this.FilePath);
                            uploader.Upload("http://127.0.0.1:8000/file/upload/", this.FilePath);
                        }
                    }
                };
            }
        }

    }
}
