using MongoDB.Driver;

namespace MongoMQTest.DB
{
    public class CollectionBase
    {
        private const string db_name = "JSMessageQueue";

        protected IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            MongoClient client = new MongoClient(new MongoUrl(AppSettings.MessageConnectionString));
            IMongoDatabase db = client.GetDatabase(db_name);
            return db.GetCollection<T>(collectionName);
        }
    }
}
