using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoMQTest.Queue;
using System;

namespace MongoMQTest.Model
{
    public enum TaskStatus
    {
        Processing = 0,
        Succeed = 1,
        Failed = 2,
        Canceled = 3
    }

    public class AnalysisTask : MsgBase
    {
        public string MsgId { get; set; }

        public string FilePath { get; set; }
        public long FileSize { get; set; }

        public long Length { get; set; }
        public long CurrentLen { get; set; }
        
        public int TaskNo { get; set; }
        public string DestFilePath { get; set; }


        [BsonRepresentation(BsonType.String)]
        public TaskStatus Status { get; set; }

        public AnalysisTask()
        {

        }

        public AnalysisTask(AnalysisMsg msg)
        {
            if (msg.IsNull())
            {
                throw new ArgumentNullException();
            }

            GroupId = msg.GroupId;
            Name = msg.Name;
            MsgId = msg.Id;
            TaskNo = msg.TaskNo;

            MsgDescription = msg.MsgDescription;
            FilePath = msg.FilePath;
            FileSize = msg.FileSize;
            Length = msg.Length;
        }

    }
}
