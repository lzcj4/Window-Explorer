using ServiceStack.Redis;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace HttpFileUploader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UploadViewModel ViewModel
        {
            get { return this.DataContext as UploadViewModel; }
        }
        RedisClient client;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new UploadViewModel();
            this.Loaded += (sender, e) =>
            {
                this.ViewModel.Load();
                //client = new RedisClient("172.16.5.75", 6379, "123456");
                //var v = client.Get<string>("AA");
                //var sub = client.CreateSubscription();
                //sub.OnSubscribe = (msg) =>
                //{
                //    Debug.WriteLine(msg);
                //};
                //sub.OnMessage = (channel, msg) =>
                //{
                //    Debug.WriteLine("channel:{0},msg:{1}".StrFormat(channel, msg));
                //};
                //sub.SubscribeToChannels("CHANNEL_1");

                //Debug.WriteLine(v);
                SendToParentProcess();
            };
            this.Closing += (sender, e) => { this.ViewModel.UnLoad(); };
        }

        private void SendToParentProcess()
        {
            Process proc = Process.GetCurrentProcess();
            //StreamWriter sw = proc.StandardInput;
            //sw.WriteLineAsync("test sub_process");
            //proc.BeginOutputReadLine();

            //StreamReader sr = proc.StandardOutput;
            //var line = sr.ReadLine();

            //MessageBox.Show(line);

        }
    }
}
