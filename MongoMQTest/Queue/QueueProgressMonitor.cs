using MongoDB.Driver;
using MongoDB.Messaging;
using MongoMQTest.DB;
using MongoMQTest.Model;
using MongoMQTest.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MongoMQTest.Queue
{
    public class AnalysisProgressArgs : EventArgs
    {
        public string FilePath { get; private set; }
        public long Length { get; private set; }
        public long Progress { get; private set; }

        public AnalysisProgressArgs(string filePath, long len, long progress)
        {
            if (filePath.IsNullOrEmpty() || len < 0 || progress < 0)
            {
                throw new InvalidOperationException();
            }

            this.FilePath = filePath;
            this.Length = len;
            this.Progress = progress;
        }
    }

    public class QueueProgressMonitor : CollectionBase, IDisposable
    {
        private const int sleepTime = 2 * 1000;
        public event EventHandler<AnalysisProgressArgs> OnProgressChanged;

        Lazy<IMongoCollection<Message>> collectionMsg;
        IMongoCollection<Message> CollectionMessage
        {
            get { return collectionMsg.Value; }
        }

        private AnalysisTaskCollection collectionTask;
        private TaskDistributedLock distLocker;
        private TaskParams taskParams;
        private bool isRunning = true;

        public QueueProgressMonitor(TaskParams taskParams)
        {
            if (taskParams.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.taskParams = taskParams;
            this.collectionTask = new AnalysisTaskCollection();
            this.distLocker = new TaskDistributedLock();
            this.collectionMsg = new Lazy<IMongoCollection<Message>>(() => GetCollection<Message>(MsgQueue.quenue_name));
        }

        private IList<string> GetMessageGroup()
        {
            var filter = Builders<Message>.Filter.Ne(m => m.State, MessageState.Complete);
            //Correlation 是消息的GroupId
            IList<string> msgGroups = this.CollectionMessage.Distinct<string>("Correlation", filter).ToList();
            return msgGroups;
        }

        private IList<string> lastGroups;
        private void GetProgress()
        {
            while (isRunning)
            {
                IList<string> currentGroups = new List<string>();
                IList<string> groupIds = this.GetMessageGroup();
                
                foreach (var item in groupIds)
                {
                    //只显示自己发送任务的进度
                    if (!MsgQueue.Instance.IsSendBySelf(item))
                    {
                        continue;
                    }

                    if (!currentGroups.Contains(item))
                    {
                        currentGroups.Add(item);
                    }
                    RaiseProgressByMsg(item);
                }

                //如果MSG已经完成状，没法取到MSG信息，最后TASK进度就没法体现
                //所以在此做一次补偿进度查询
                if (!lastGroups.IsNullOrEmpty())
                {
                    var completedGroups = lastGroups.Except(currentGroups);
                    foreach (var item in completedGroups)
                    {
                        RaiseProgressByMsg(item);
                    }
                }

                lastGroups = currentGroups;
                Thread.Sleep(sleepTime);
            }
        }

        private void RaiseProgressByMsg(string groupId)
        {
            IList<AnalysisTask> tasks = collectionTask.GetItems(groupId);
            if (tasks.IsNullOrEmpty())
            {
                return;
            }

            long currentProgress = tasks.Sum(t => t.CurrentLen);
            AnalysisTask task = tasks.FirstOrDefault();
            long totalSize = task.FileSize;

            this.CheckLocker(groupId, currentProgress / totalSize, tasks);
            this.RaiseProgressChanged(task.FilePath, totalSize, currentProgress);
        }

        public void Start()
        {
            Task.Run(new Action(this.GetProgress));
        }

        public void Stop()
        {
            this.isRunning = false;
        }

        private void RaiseProgressChanged(string filePath, long len, long pos)
        {
            var progressEvent = OnProgressChanged;
            if (!progressEvent.IsNull())
            {
                progressEvent(this, new AnalysisProgressArgs(filePath, len, pos));
            }
        }

        private void CheckLocker(string groupId, double progress, IList<AnalysisTask> tasks)
        {
            if (groupId.IsNullOrEmpty() || progress != 1.0 || tasks.IsNullOrEmpty())
            {
                return;
            }

            Task.Run(() =>
            {
                MsgQueue.Instance.RemoveSucceedGroupId(groupId);
                bool isAcquired = distLocker.Acquire(groupId);
                if (isAcquired)
                {
                    try
                    {
                        this.SendMergeTask(tasks);
                    }
                    finally
                    {
                        distLocker.Release(groupId);
                    }
                }
            });
        }

        private void SendMergeTask(IList<AnalysisTask> tasks)
        {
            if (tasks.IsNullOrEmpty())
            {
                return;
            }

            AnalysisTask firstTask = tasks.FirstOrDefault();
            string destPath = Path.Combine(taskParams.DestFolder, Path.GetFileName(firstTask.FilePath));
            IList<string> filesPath = tasks.OrderBy(t => t.TaskNo).Select(t => t.DestFilePath).ToList();

            FileMergeMsg mergeMsg = new FileMergeMsg() { DestPath = destPath, TempFiles = filesPath };
            MsgQueue.Instance.Publish(mergeMsg);
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
