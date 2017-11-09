using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader.Tasks
{
    public interface ITask : IDisposable
    {
        TaskGroup Group { get; set; }
        TaskPriority Priority { get; set; }

        bool IsCompleted { get; set; }

        void Load();

        void Run();

        void Pause();

        void Stop();

        void UnLoad();
    }

    public abstract class TaskBase : ITask
    {
        public TaskGroup Group { get; set; }

        public TaskPriority Priority { get; set; }

        public bool IsCompleted { get; set; }

        public virtual void Load()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void Pause()
        {
            throw new NotImplementedException();
        }

        protected bool isRunning = true;
             
        public virtual void Stop()
        {
            throw new NotImplementedException();
        }

        public virtual void UnLoad()
        {
        }


        #region IDisposable

        private bool isDisposed = false;
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }
            this.isDisposed = true;
            OnDisposing(this.isDisposed);
        }

        protected virtual void OnDisposing(bool isDisposed)
        {
        }

        #endregion
    }

}
