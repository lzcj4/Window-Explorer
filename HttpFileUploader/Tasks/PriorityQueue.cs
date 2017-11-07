using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader.Tasks
{
    public enum TaskPriority
    {
        Low, Noraml, High
    }

    public class PriorityQueue
    {
        private IDictionary<TaskPriority, Queue<ITask>> TaskDic { get; set; }

        public PriorityQueue()
        {
            TaskDic = new Dictionary<TaskPriority, Queue<ITask>>();
            TaskDic[TaskPriority.Low] = new Queue<ITask>();
            TaskDic[TaskPriority.Noraml] = new Queue<ITask>();
            TaskDic[TaskPriority.High] = new Queue<ITask>();
        }

        public void Enqueue(ITask task)
        {
            if (task.IsNull())
            {
                throw new ArgumentNullException();
            }

            TaskDic[task.Priority].Enqueue(task);
        }

        public ITask Dequeue(TaskPriority priority)
        {
            var queue = TaskDic[priority];
            ITask result = queue.Peek();
            if (!result.IsNull())
            {
                result = queue.Dequeue();
            }
            return result;
        }
    }
}
