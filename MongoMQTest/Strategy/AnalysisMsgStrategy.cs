using MongoMQTest.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace MongoMQTest
{
    /// <summary>
    /// 按文件大小平均等分
    /// </summary>
    class AnalysisMsgStrategy : IMsgStrategy
    {
        //1 GB
        private const int fileSize = 1 * 1024 * 1024;

        private string FilePath { get; set; }
        private int ServerCount { get; set; }

        public AnalysisMsgStrategy()
        {

        }

        public void Initial(string filePath, int serverCount = 1)
        {
            if (filePath.IsNullOrEmpty() || serverCount < 0)
            {
                throw new InvalidOperationException();
            }
            this.FilePath = filePath;
            this.ServerCount = serverCount;
        }

        public IList<MsgBase> Split()
        {
            if (!File.Exists(this.FilePath))
            {
                throw new FileNotFoundException(FilePath);
            }

            IList<MsgBase> result = new List<MsgBase>();
            long fileLen = new FileInfo(this.FilePath).Length;
            if (fileLen <= AnalysisMsgStrategy.fileSize)
            {
                AnalysisMsg msg = new AnalysisMsg(this.FilePath, fileLen);
                result.Add(msg);
            }
            else
            {
                long sliceLen = fileLen / this.ServerCount;
                long curretPos = 0;
                long currentLen = 0;
                string groupId = MsgBase.NewGuid();
                string fileName = Path.GetFileNameWithoutExtension(this.FilePath);

                for (int i = 0; i < ServerCount; i++)
                {
                    curretPos += currentLen;
                    if (i == ServerCount - 1)
                    {
                        currentLen = fileLen - curretPos;
                    }
                    else
                    {
                        currentLen = sliceLen;
                    }

                    AnalysisMsg msg = new AnalysisMsg(this.FilePath, fileLen)
                    {
                        GroupId = groupId,
                        TaskNo = i + 1,
                        TaskCount = ServerCount,
                        StartPos = curretPos,
                        Length = currentLen,
                        MsgDescription = MsgType.AnalysisMsg.ToString(),
                        Name = string.Format("{0}_{1}", fileName, i + 1)
                    };
                    result.Add(msg);
                }
            }
            return result;
        }
        
    }
}
