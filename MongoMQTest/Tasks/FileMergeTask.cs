using MongoMQTest.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace MongoMQTest.Tasks
{
    class FileMergeTask : TaskBase
    {
        public override bool Run()
        {
            this.Merge(this.destPath, this.filesPath);
            return true;
        }

        private string destPath;
        private IList<string> filesPath;

        public void Initial(FileMergeMsg msg)
        {
            if (msg.IsNull() || msg.DestPath.IsNullOrEmpty() ||
               msg.TempFiles.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            this.destPath = msg.DestPath;
            this.filesPath = msg.TempFiles;
        }

        private void Merge(string destPath, IList<string> filesList)
        {
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            //1MB
            int bufferLen = 1 * 1024 * 1024;
            byte[] buffer = new byte[bufferLen];
            int readLen = 0;
            //100MB
            int flushMax = 50;
            int readCount = 0;

            using (FileStream writer = new FileStream(destPath, FileMode.CreateNew, FileAccess.Write))
            {
                foreach (var filePath in filesList)
                {
                    using (FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        while (this.isRunning)
                        {
                            int len = reader.Read(buffer, 0, bufferLen);
                            if (len > 0)
                            {
                                writer.Write(buffer, 0, len);
                                readLen += len;
                                if (readCount++ > flushMax)
                                {
                                    writer.FlushAsync();
                                    readCount = 0;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    File.Delete(filePath);
                }
            }
        }
    }
}
