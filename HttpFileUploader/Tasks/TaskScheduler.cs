using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader.Tasks
{
    public class TaskScheduler
    {
        private const int DefaultThreadNum = 4;


        private bool isRunning = true;
        private PriorityQueue TaskQueue { get; set; }

        public TaskScheduler()
        {
            TaskQueue = new PriorityQueue();
        }

        #region Task Add / Remove

        public void Enqueue(ITask task)
        {
            this.TaskQueue.Enqueue(task);
        }

        public ITask Dequeue()
        {
            ITask result = null;

            TaskPriority[] priorities = { TaskPriority.High, TaskPriority.Noraml, TaskPriority.Low };
            foreach (var item in priorities)
            {
                result = this.TaskQueue.Dequeue(TaskPriority.High);
                if (!result.IsNull())
                {
                    break;
                }
            }

            return result;
        }

        #endregion

        #region Task Run


        #endregion
        
    }
}
