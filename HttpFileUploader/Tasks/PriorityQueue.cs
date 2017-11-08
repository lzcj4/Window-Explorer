using System;
using System.Collections.Generic;

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
            ITask result = null;
            var queue = TaskDic[priority];
            if (!queue.IsNullOrEmpty())
            {
                result = queue.Dequeue();
            }
            return result;
        }
    }
}
