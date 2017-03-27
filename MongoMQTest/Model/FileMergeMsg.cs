using System.Collections.Generic;

namespace MongoMQTest.Model
{
    public class FileMergeMsg : MsgBase
    {
        public string DestPath { get; set; }
        public IList<string> TempFiles { get; set; }

        public FileMergeMsg()
        {
            this.MsgType = MsgType.FileMergeMsg;
        }
    }
}
