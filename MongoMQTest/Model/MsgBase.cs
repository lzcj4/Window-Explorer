using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace MongoMQTest
{
    public enum MsgType
    {
        AnalysisMsg = 0,
        FileMergeMsg = 1
    }

    public class MsgBase
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string NewObjectId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        private string id = NewObjectId();

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get { return id; } set { this.id = value; } }

        private string groupId = MsgBase.NewGuid();
        public string GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        public string Description { get; set; }

        /// <summary>
        /// File Name + Task Id
        /// </summary>
        public string Name { get; set; }

        public MsgType MsgType { get; set; }


    }

}
