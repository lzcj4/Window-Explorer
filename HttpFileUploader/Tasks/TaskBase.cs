using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader.Tasks
{
    public interface ITask
    {
        TaskPriority Priority { get; set; }
        bool IsCompleted { get; set; }

        void Load();

        void Run();

        void UnLoad();
    }

    public abstract class TaskBase : ITask
    {
        public TaskPriority Priority { get; set; }

        public bool IsCompleted
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }
    }

}
