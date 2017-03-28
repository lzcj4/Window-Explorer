using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Messaging;
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

        /// <summary>
        /// 消息描述，主要是哪类消息
        /// </summary>
        public string MsgDescription { get; set; }

        /// <summary>
        /// File Name + Task Id
        /// </summary>
        public string Name { get; set; }

        private MessagePriority priority = MessagePriority.Normal;
        public MessagePriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
