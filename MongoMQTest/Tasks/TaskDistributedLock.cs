using MongoDB.Messaging.Locks;
using MongoMQTest.DB;
using System.Diagnostics;

namespace MongoMQTest.Tasks
{
    class TaskDistributedLock : CollectionBase
    {
        private DistributedLock locker;
        public TaskDistributedLock()
        {
            var collection = this.GetCollection<LockData>("TaskLock");
            locker = new DistributedLock(collection);
        }

        public bool Acquire(string lockName)
        {
            bool isAcquired = locker.Acquire(lockName);
            Debug.WriteLine("/++++ Locker:{0} ,Acquired:{1}  ++++/", lockName, isAcquired);
            return isAcquired;
        }

        public void Release(string lockName)
        {
            locker.Release(lockName);
            Debug.WriteLine(string.Format("/---- Release Locker:{0} ----/", lockName));
        }
        
    }
}
