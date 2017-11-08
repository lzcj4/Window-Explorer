using HttpFileUploader.Util;
using System.Collections.Generic;
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

        private object lockObj = new object();
        private AutoResetEvent taskResetEvent = new AutoResetEvent(true);

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


        public void Run()
        {
            this.isRunning = true;
            for (int i = 0; i < GlobalConst.ThreadCount; i++)
            {
                Thread t = new Thread(InternalRun);
                t.Start();
            }

            RunTaskCreateThread();
        }

        public void Stop()
        {
            this.isRunning = true;
            using (taskResetEvent)
            {
                taskResetEvent.Set();
            }
            using (taskCreateResetEvent)
            {
                taskCreateResetEvent.Set();
            }
        }

        private void InternalRun()
        {
            while (isRunning)
            {
                ITask task = null;
                lock (lockObj)
                {
                    task = this.Dequeue();
                }
                if (task.IsNull())
                {
                    taskResetEvent.WaitOne();
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
                    ChunkFileTaskGroup group = new ChunkFileTaskGroup(filePaths);
                    group.Create();
                    foreach (ChunkFileTaskGroup subGroup in group.Groups)
                    {
                        taskGroupDict[subGroup.FilePath] = subGroup;
                    }

                    foreach (var t in group.AllTasks)
                    {
                        TaskQueue.Enqueue(t);
                    }
                }
            });
            taskCreateThread.Start();
        }

        public void AddFile(params string[] filePaths)
        {
            fileQueue.Enqueue(filePaths);
            taskCreateResetEvent.Set();
        }

        #endregion

        #region Task Run


        #endregion

    }
}
