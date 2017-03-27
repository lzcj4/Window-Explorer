namespace MongoMQTest.Tasks
{
    public interface ITask
    {
        bool Run();

        void Stop();
    }
}
