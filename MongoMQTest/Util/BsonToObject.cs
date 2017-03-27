using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoMQTest.Util
{
    class BsonToObject
    {
        public static T Convert<T>(BsonDocument bson)
        {
            return BsonSerializer.Deserialize<T>(bson);
        }
    }
}
