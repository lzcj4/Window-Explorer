using MongoMQTest.DB;
using MongoMQTest.Model;
using MongoMQTest.Queue;
using System;
using System.Diagnostics;
using System.IO;

namespace MongoMQTest.Tasks
{
    class FileCopyTask : TaskBase
    {
        AnalysisMsg Msg { get; set; }
        TaskParams taskParams { get; set; }

        public FileCopyTask()
        {
        }

        public void Initial(AnalysisMsg msg, TaskParams param)
        {
            if (msg.IsNull() || param.IsNull())
            {
                throw new InvalidOperationException("Initial failed with null arguments");
            }
            this.Msg = msg;
            this.taskParams = param;
        }

        public override bool Run()
        {
            bool result = false;
            if (!File.Exists(this.Msg.FilePath))
            {
                return result;
            }

            string destPath = Path.Combine(taskParams.TempFolder, this.Msg.Name + ".temp");
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }

            AnalysisTask task = new AnalysisTask(this.Msg)
            {
                CurrentLen = 0,
                DestFilePath = destPath
            };

            //1MB
            int bufferLen = 1 * 1024 * 1024;
            byte[] buffer = new byte[bufferLen];
            long totalReadLen = this.Msg.Length;
            int readLen = 0;
            //100MB
            int flushMax = 50;
            int readCount = 0;

            try
            {
                using (FileStream readStream = new FileStream(this.Msg.FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream writeStream = new FileStream(destPath, FileMode.CreateNew, FileAccess.Write))
                    {
                        readStream.Seek(this.Msg.StartPos, SeekOrigin.Begin);
                        while (this.isRunning)
                        {
                            int len = readStream.Read(buffer, 0, bufferLen);
                            if (len > 0)
                            {
                                writeStream.Write(buffer, 0, len);
                                readLen += len;
                                if (totalReadLen - readLen < bufferLen)
                                {
                                    bufferLen = (int)(totalReadLen - readLen);
                                }
                                if (readCount++ > flushMax)
                                {
                                    writeStream.FlushAsync();
                                    readCount = 0;
                                    task.CurrentLen = readLen;
                                    AnalysisTaskCollection.Instance.Save(task);
                                }
                            }
                            else
                            {
                                task.CurrentLen = readLen;
                                task.Status = TaskStatus.Succeed;
                                AnalysisTaskCollection.Instance.Save(task);
                                result = true;
                                break;
                            }
                            Array.Clear(buffer, 0, bufferLen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("/*** Read and write file failed", ex.Message);
                task.CurrentLen = readLen;
                task.Status = TaskStatus.Failed;
                AnalysisTaskCollection.Instance.Save(task);
                result = false;
            }

            return result;
        }
    }
}
