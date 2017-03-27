using MongoDB.Driver;
using MongoMQTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoMQTest.DB
{
    public class AnalysisTaskCollection : CollectionBase
    {
        public const string collection_task_name = "Tasks";

        private static AnalysisTaskCollection instance;
        public static AnalysisTaskCollection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (collection_task_name)
                    {
                        instance = new AnalysisTaskCollection();
                    }
                }
                return instance;
            }
        }

        private readonly Lazy<IMongoCollection<AnalysisTask>> taskCollection;
        public IMongoCollection<AnalysisTask> Tasks
        {
            get { return taskCollection.Value; }
        }

        public AnalysisTaskCollection()
        {
            taskCollection = new Lazy<IMongoCollection<AnalysisTask>>(() => GetCollection<AnalysisTask>(collection_task_name));
        }

        public AnalysisTask Save(AnalysisTask task)
        {
            if (task.IsNull())
                throw new ArgumentNullException("task");

            var updateOptions = new UpdateOptions { IsUpsert = true };

            return this.Tasks
                .ReplaceOneAsync(t => t.Id == task.Id, task, updateOptions)
                .ContinueWith(t => task).Result;
        }

        public IList<AnalysisTask> GetItems(string groupId)
        {
            if (groupId.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            var filter = Builders<AnalysisTask>.Filter.Eq(t => t.GroupId, groupId);
            return this.Tasks.Find<AnalysisTask>(filter).ToListAsync<AnalysisTask>().Result;
        }

        public IEnumerable<IGrouping<string, AnalysisTask>> GetGroup(string groupId)
        {
            if (groupId.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            return GetItems(groupId).GroupBy(item => item.GroupId);
        }

    }
}
