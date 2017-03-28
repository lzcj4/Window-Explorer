using MongoDB.Messaging;
using System.Collections.Generic;

namespace MongoMQTest.Model
{
    public class FileMergeMsg : MsgBase
    {
        public string DestPath { get; set; }
        public IList<string> TempFiles { get; set; }

        public FileMergeMsg()
        {
            this.MsgDescription = MsgType.FileMergeMsg.ToString();
            this.Priority = MessagePriority.High;
        }
    }
}
