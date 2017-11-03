using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpFileUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TestFileRead();
            this.DataContext = new UploadViewModel();
        }

        private void TestFileRead()
        {
            string filePath = @"D:\软件\飞秋FeiQ.exe";

            Task t1 = Task.Run(() =>
            {
                var stream = ShareReadFileApi.OpenShareReadFile(filePath);
                //stream.Seek(100, SeekOrigin.Begin);
                stream.ReadByte();
                var startPos = stream.Position;
            });
            Task t2 = Task.Run(() =>
            {
                var stream = ShareReadFileApi.OpenShareReadFile(filePath);
                var startPos = stream.Position;
                Thread.Sleep(3 * 1000);
                var endPos = stream.Position;
            });
            Task.WhenAll(t1, t2);
        }
    }


}
