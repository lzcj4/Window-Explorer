namespace MongoMQTest.Tasks
{
    public abstract class TaskBase : ITask
    {
        public abstract bool Run();

        protected bool isRunning = true;
        public void Stop()
        {
            isRunning = false;
        }
    }
}
