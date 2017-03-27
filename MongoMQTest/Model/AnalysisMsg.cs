using System;
using System.IO;

namespace MongoMQTest.Model
{
    public class AnalysisMsg : MsgBase
    {
        public int TaskNo { get; set; }
        public int TaskCount { get; set; }

        public string FilePath { get; set; }
        public long FileSize { get; set; }

        ///分片起始位
        public long StartPos { get; set; }

        private long length;

        ///分片文件长度
        public long Length
        {
            get { return length <= 0 ? this.FileSize : length; }
            set { length = value; }
        }


        public AnalysisMsg(string filePath, long fileSize)
        {
            if (filePath.IsNullOrEmpty() || !File.Exists(filePath))
            {
                throw new InvalidOperationException(filePath);
            }

            this.MsgType = MsgType.AnalysisMsg;
            this.FilePath = filePath;
            this.FileSize = fileSize;
            if (fileSize <= 0)
            {
                this.FileSize = new FileInfo(filePath).Length;
            }
        }

    }
}
