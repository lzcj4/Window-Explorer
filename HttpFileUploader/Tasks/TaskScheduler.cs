using HttpFileUploader.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace HttpFileUploader.Tasks
{
    public class TaskScheduler
    {
        private static TaskScheduler instance;
        public static TaskScheduler Instance
        {
            get
            {
                instance = instance ?? new TaskScheduler();
                return instance;
            }
        }

        public event EventHandler<FileUploadProgressEventArgs> OnUploading;

        private object lockObj = new object();

        private bool isRunning = true;
        private PriorityQueue TaskQueue { get; set; }

        public TaskScheduler()
        {
            TaskQueue = new PriorityQueue();
        }

        #region Task Add / Remove

        private void Enqueue(ITask task)
        {
            this.TaskQueue.Enqueue(task);
        }

        private ITask Dequeue()
        {
            lock (this.lockObj)
            {
                ITask result = null;
                TaskPriority[] priorities = { TaskPriority.High, TaskPriority.Noraml, TaskPriority.Low };
                foreach (var item in priorities)
                {
                    result = this.TaskQueue.Dequeue(item);
                    if (!result.IsNull())
                    {
                        break;
                    }
                }

                return result;
            }
        }

        private int ThreadCount
        {
            get { return GlobalConst.ThreadCount; }
        }

        IList<TaskThread> taskThreads = new List<TaskThread>();
        public void Run()
        {
            this.isRunning = true;
            for (int i = 0; i < this.ThreadCount; i++)
            {
                TaskThread tt = new TaskThread(this);
                tt.Start();
                taskThreads.Add(tt);
            }

            RunTaskCreateThread();
        }

        private void ResumeTaskThread()
        {
            foreach (var item in taskThreads)
            {
                item.Resume();
            }
        }

        public void Stop()
        {
            this.isRunning = false;
            using (taskCreateResetEvent)
            {
                taskCreateResetEvent.Set();
            }

            foreach (var item in taskThreads)
            {
                item.Stop();
            }
            taskThreads.Clear();
        }

        #endregion

        #region Task Creat Thread

        private Thread taskCreateThread;
        private AutoResetEvent taskCreateResetEvent = new AutoResetEvent(true);
        private IDictionary<string, TaskGroup> taskGroupDict = new Dictionary<string, TaskGroup>();
        private Queue<string[]> fileQueue = new Queue<string[]>();

        private void RunTaskCreateThread()
        {
            taskCreateThread = new Thread(() =>
            {
                while (isRunning)
                {
                    if (fileQueue.IsNullOrEmpty())
                    {
                        taskCreateResetEvent.WaitOne();
                        continue;
                    }

                    string[] filePaths = fileQueue.Dequeue();
                    ///multi-file only create multi sub groups, no tasks
                    ChunkFileTaskGroup group = new ChunkFileTaskGroup(filePaths);
                    group.OnUploading += File_OnUploading;
                    group.Create();

                    foreach (ChunkFileTaskGroup subGroup in group.Groups)
                    {
                        taskGroupDict[subGroup.FilePath] = subGroup;
                    }

                    foreach (var t in group.AllTasks)
                    {
                        TaskQueue.Enqueue(t);
                    }
                    ResumeTaskThread();
                }
            });
            taskCreateThread.Start();
        }

        private void File_OnUploading(object sender, FileUploadProgressEventArgs e)
        {
            var uploadEvt = this.OnUploading;
            if (!uploadEvt.IsNull())
            {
                uploadEvt(this, e);
                if ((sender is TaskGroup) && e.Progress >= e.Len)
                {
                    AddChunkConcatTast(e.FilePath);
                }
            }
        }

        private void AddChunkConcatTast(string filePath)
        {
            if (filePath.IsNullOrEmpty() || !filePath.IsFileExisted())
            {
                return;
            }
            ChunkConcatTask task = new ChunkConcatTask(filePath) { Priority = TaskPriority.High };
            task.OnUploading += File_OnUploading;

            TaskQueue.Enqueue(task);
        }

        public void AddFile(params string[] filePaths)
        {
            fileQueue.Enqueue(filePaths);
            taskCreateResetEvent.Set();
        }

        #endregion


        private class TaskThread
        {
            public TaskScheduler Parent { get; set; }
            private Thread thread;
            private AutoResetEvent threadResetEvent = new AutoResetEvent(true);
            private bool isRunning = true;

            public TaskThread(TaskScheduler parent)
            {
                this.Parent = parent;
            }

            public void Start()
            {
                this.isRunning = true;
                thread = new Thread(InternalRun);
                thread.Start();
            }

            public void Resume()
            {
                this.threadResetEvent.Set();
            }

            private void InternalRun()
            {
                while (this.isRunning)
                {
                    ITask task = Parent.Dequeue();
                    if (task.IsNull())
                    {
                        threadResetEvent.WaitOne();
                        continue;
                    }

                    using (task)
                    {
                        task.Load();
                        task.Run();
                        task.UnLoad();
                    }
                }
            }

            public void Stop()
            {
                this.isRunning = false;
                this.Resume();
                thread.Join();
                thread = null;
            }
        }
    }

}
