using MongoMQTest.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MongoMQTest.Tasks
{
    public class TaskScheduler : IDisposable
    {
        private static object lockObj = new object();

        private static TaskScheduler instance;
        public static TaskScheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        instance = new TaskScheduler();
                    }
                }
                return instance;
            }
        }

        private AutoResetEvent autoResetEvent;

        private int serverCount = 1;
        private bool isRunning = true;

        Queue<ITask> queue;

        private TaskScheduler()
        {
            autoResetEvent = new AutoResetEvent(false);
            queue = new Queue<ITask>();
            serverCount = AppSettings.SeverCount;
        }

        private void StartHandleThread()
        {
            while (isRunning)
            {
                ITask task = null;
                lock (TaskScheduler.lockObj)
                {
                    if (queue.Count != 0)
                    {
                        task = queue.Dequeue();
                    }
                }

                if (task.IsNull())
                {
                    autoResetEvent.WaitOne();
                    continue;
                }

                try
                {
                    task.Run();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Task:{0} invoked failed:{1}", typeof(ITask), ex.Message);
                }
            }
        }

        public void Add(ITask task)
        {
            if (task.IsNull())
            {
                return;
            }

            lock (TaskScheduler.lockObj)
            {
                this.queue.Enqueue(task);
                autoResetEvent.Set();
            }
        }
        
        public void Start()
        {
            Task.Run(() => { this.StartHandleThread(); });
        }

        public void Stop()
        {
            this.isRunning = false;
            while (queue.Count != 0)
            {
                var task = queue.Dequeue();
                task.Stop();
            }

            if (autoResetEvent != null)
            {
                using (autoResetEvent)
                {
                    autoResetEvent.Set();
                    autoResetEvent.Close();
                }
            }
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
