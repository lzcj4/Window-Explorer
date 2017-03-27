using MongoMQTest.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MongoMQTest
{
    public class AnalysisMsgScheduler : IDisposable
    {
        private static object lockObj = new object();

        private static AnalysisMsgScheduler instance;
        public static AnalysisMsgScheduler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        instance = new AnalysisMsgScheduler();
                    }
                }
                return instance;
            }
        }

        private AutoResetEvent autoResetEvent;

        private int serverCount = 1;
        private bool isRunning = true;
        private Queue<string> queue;

        private AnalysisMsgScheduler()
        {
            autoResetEvent = new AutoResetEvent(false);
            serverCount = AppSettings.SeverCount;
            queue = new Queue<string>();
        }

        private void StartHandleThread()
        {
            while (isRunning)
            {
                string item = string.Empty;
                lock (AnalysisMsgScheduler.lockObj)
                {
                    if (queue.Count != 0)
                    {
                        item = queue.Dequeue();
                    }
                }

                if (item.IsNullOrEmpty())
                {
                    autoResetEvent.WaitOne();
                    continue;
                }

                AnalysisMsgStrategy strategy = new AnalysisMsgStrategy();
                strategy.Initial(item, serverCount);
                IEnumerable<AnalysisMsg> slices = strategy.Split();

                MsgQueue.Instance.Publish(slices);
            }
        }

        public void Add(string filePath)
        {
            if (filePath.IsNullOrEmpty())
            {
                return;
            }

            lock (AnalysisMsgScheduler.lockObj)
            {
                this.queue.Enqueue(filePath);
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
