using MongoMQTest.Model;
using MongoMQTest.Queue;
using MongoMQTest.Tasks;
using System;
using System.Windows.Input;

namespace MongoMQTest.ViewModel
{
    public class AnalysisViewModel : ViewModelBase
    {
        #region DP

        private string filePath = @"E:\Test\cn_visio_professional_2016_x86_x64_dvd_6970929.iso";
        public string FilePath
        {
            get { return filePath; }
            set { this.SetProperty<string>(ref filePath, value, "FilePath"); }
        }

        private string tempFolder = AppSettings.TempFolder;
        public string TempFolder
        {
            get { return tempFolder; }
            set { this.SetProperty<string>(ref tempFolder, value, "TempFolder"); }
        }

        private string destFolder = AppSettings.DestinationFolder;
        public string DestFolder
        {
            get { return destFolder; }
            set { this.SetProperty<string>(ref destFolder, value, "DestFolder"); }
        }

        private string elapsedTime;
        public string ElapsedTime
        {
            get { return elapsedTime; }
            set { this.SetProperty<string>(ref elapsedTime, value, "ElapsedTime"); }
        }

        private double progress = 0;
        public double Progress
        {
            get { return progress; }
            set { this.SetProperty<double>(ref progress, value, "Progress"); }
        }

        public ICommand SendCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) => { this.SendMsg(); }
                };
            }
        }

        public ICommand ProcessCommand
        {
            get
            {
                return new GenericCommand()
                {
                    CanExecuteCallback = (obj) => { return true; },
                    ExecuteCallback = (obj) => { this.ProcessTask(); }
                };
            }
        }

        #endregion

        QueueProgressMonitor monitor;
        AnalysisMsgScheduler msgScheduler;
        MsgQueue msgQueue;
        TaskScheduler taskScheduler;

        private void SendMsg()
        {
            ProcessTask();
            msgScheduler = AnalysisMsgScheduler.Instance;
            msgScheduler.Start();
            msgScheduler.Add(this.FilePath);
            StartMonitor();
        }

        private void ProcessTask()
        {
            taskScheduler = TaskScheduler.Instance;
            taskScheduler.Start();

            msgQueue = MsgQueue.Instance;
            msgQueue.Subscribe();
            msgQueue.OnMsgReceived += (sender, e) =>
            {
                MsgBase msg = e.Msg;
                if (msg is AnalysisMsg)
                {
                    //同步处理，防止消息和TASK状态不一
                    FileCopyTask task = new FileCopyTask();
                    task.Initial(e.Msg as AnalysisMsg, this.GetParams());
                    task.Run();
                }
                else if (msg is FileMergeMsg)
                {
                    FileMergeTask task = new FileMergeTask();
                    task.Initial(e.Msg as FileMergeMsg);
                    task.Run();
                }
            };

            this.StartMonitor();
        }

        private TaskParams GetParams()
        {
            TaskParams result = new TaskParams();
            result.FilePath = this.FilePath;
            result.TempFolder = this.TempFolder;
            result.DestFolder = this.DestFolder;
            return result;
        }

        private DateTime startDate = DateTime.Now;
        private bool isProgessStart = false;
        private void StartMonitor()
        {
            this.StopMonitor();

            monitor = new QueueProgressMonitor(GetParams());
            monitor.OnProgressChanged += (sender, e) =>
            {
                if (e.Length <= 0)
                {
                    return;
                }

                this.RunOnUIThreadAsync(() =>
                {
                    this.Progress = Math.Floor(100.0 * e.Progress / e.Length);
                    if (this.Progress == 100 && isProgessStart)
                    {
                        this.ElapsedTime = string.Format("总用时:{0}秒", DateTime.Now.Subtract(startDate).TotalSeconds);
                        isProgessStart = false;
                        return;
                    }

                    if (!isProgessStart && this.Progress > 0)
                    {
                        this.ElapsedTime = "开始记时";
                        startDate = DateTime.Now;
                        isProgessStart = true;
                    }
                });
            };
            monitor.Start();
        }

        private void StopMonitor()
        {
            if (!this.monitor.IsNull())
            {
                this.monitor.Dispose();
                this.monitor = null;
            }
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
            if (!msgQueue.IsNull())
            {
                msgQueue.Dispose();
                msgQueue = null;
            }
            if (!msgScheduler.IsNull())
            {
                msgScheduler.Dispose();
                msgScheduler = null;
            }
            if (!taskScheduler.IsNull())
            {
                taskScheduler.Dispose();
                taskScheduler = null;
            }
            this.StopMonitor();
        }
    }
}
