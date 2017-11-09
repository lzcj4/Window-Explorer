using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace HttpFileUploader
{
    public class ChunkUploadProgressEventArgs : EventArgs
    {
        public string FileName { get; set; }

        /// <summary>
        /// File total chunk count
        /// </summary>
        public int ChunkCount { get; set; }

        /// <summary>
        /// Current chunk no 
        /// </summary>
        public int ChunkNo { get; set; }

        /// <summary>
        /// Total file length
        /// </summary>
        public long Len { get; set; }

        /// <summary>
        /// Total read bytes
        /// </summary>
        public long Progress { get; set; }

        /// <summary>
        /// Current read bytes
        /// </summary>
        public long ReadLen { get; set; }

        public ChunkUploadProgressEventArgs(FileChunkItem fileItem, long readLen, long progress) :
            this(fileItem.FileName, fileItem.ChunkCount, fileItem.ChunkNo,
                fileItem.ChunkLen, readLen, progress)
        {
        }

        public ChunkUploadProgressEventArgs(string fileName, int chunkCount, int chunkNo,
            long len, long readLen, long progress)
        {
            if (fileName.IsNullOrEmpty() ||
                chunkCount < 0 || chunkNo < 0 || chunkNo > chunkCount ||
                      len < 0 || chunkNo < 0 || progress < 0 ||
                      readLen > len || progress > len)
            {
                throw new ArgumentException();
            }
            this.FileName = fileName;
            this.ChunkCount = chunkCount;
            this.ChunkNo = chunkNo;
            this.Len = len;
            this.ReadLen = readLen;
            this.Progress = progress;
        }
    }

    public class ChunkFileHttp : HttpBase
    {
        public const string FOLDERNAME = "foldername";
        public const string FILENAME = "filename";
        public const string FILE_INFOS = "fileinfos";

        public const string CHUNK_COUNT = "chunkcount";
        public const string CHUNK_NO = "chunkno";
        public const string CHUNK_LEN = "chunklen";

        private const string CODE = "code";
        private const string MSG = "msg";
        private const string CODE_SUCCEED = "200";

        private string WebHost = "http://127.0.0.1:8000/";
        private string ListUrl { get { return Path.Combine(WebHost, "file/list/"); } }
        private string UploadUrl { get { return Path.Combine(WebHost, "file/upload/"); } }
        private string ConcatUrl { get { return Path.Combine(WebHost, "file/concat/"); } }

        private FileChunkItem FileItem { get; set; }

        public event EventHandler<ChunkUploadProgressEventArgs> OnUploading;

        public ChunkFileHttp()
        {
        }

        public ChunkFileHttp(string host)
        {
            WebHost = "http://{0}/".StrFormat(host);
        }

        public IDictionary<string, long> List(string folderName)
        {
            IDictionary<string, long> dic = new Dictionary<string, long>();
            JObject postData = new JObject();
            postData[ChunkFileHttp.FOLDERNAME] = folderName;
            var request = PostRequest(ListUrl, postData.ToString());
            //var request = GetRequest(QueryUrl + folderName);
            using (var resposne = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resposne.GetResponseStream()))
                {
                    var uploadResult = sr.ReadToEnd();
                    Debug.WriteLine(uploadResult);
                    JObject jObj = JObject.Parse(uploadResult);
                    if (jObj[ChunkFileHttp.CODE].ToString() == ChunkFileHttp.CODE_SUCCEED)
                    {
                        var list = jObj[ChunkFileHttp.FILE_INFOS];
                        foreach (var item in list)
                        {
                            dic[item[ChunkFileHttp.FILENAME].ToString()] = long.Parse(item[ChunkFileHttp.CHUNK_LEN].ToString());
                        }
                    }
                }
            }
            return dic;
        }

        public bool Upload(FileChunkItem fileItem)
        {
            if (fileItem.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.FileItem = fileItem;
            HttpWebRequest request = CreateRequest(UploadUrl, HttpMethod.Post);
            //request.SendChunked = true;
            request.AllowWriteStreamBuffering = false;
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            request.Proxy = this.GetProxy();
            Guid guid = new Guid();
            string boundary = "----{0}".StrFormat(guid);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "------{0}\r\n".StrFormat(guid);
            string fileInfoHeader = "\r\n{0}Content-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}".StrFormat(boundary, FILE_INFOS, fileItem.ToJson());

            string fileHeader = ("\r\n{0}Content-Disposition: form-data; name=\"file\";filename=\"{1}\"\r\n" +
                                  "Content-Type: application/octet-stream\r\n\r\n")
                                  .StrFormat(boundary, fileItem.FileName);
            byte[] fileInfoBytes = Encoding.UTF8.GetBytes(fileInfoHeader);
            byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);

            var footerBytes = Encoding.UTF8.GetBytes("\r\n{0}".StrFormat(boundary));

            Stream fileStream = fileItem.Stream;
            request.ContentLength = fileInfoBytes.Length + fileHeaderBytes.Length + fileStream.Length + footerBytes.Length;

            byte[] buffer = new byte[512 * 1024];
            request.Proxy = this.GetProxy();
            long readBytes = 0;
            long readProgress = 0;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(fileInfoBytes, 0, fileInfoBytes.Length);
                stream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

                using (fileStream)
                {
                    int len = 0;
                    int count = 0;
                    while (this.isRunning &&
                          (len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, len);
                        readBytes += len;
                        readProgress += len;
                        if (count++ % 50 == 0)
                        {
                            stream.Flush();
                            Thread.Sleep(5);
                        }
                        if (count++ % 5 == 0)
                        {
                            RaiseUploading(readBytes, readProgress);
                            readBytes = 0;
                        }
                    }
                }
                stream.Write(footerBytes, 0, footerBytes.Length);
            }

            using (var resposne = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resposne.GetResponseStream()))
                {
                    var uploadResult = sr.ReadToEnd();
                    Debug.WriteLine(uploadResult);
                    JObject jObj = JObject.Parse(uploadResult);

                    bool isSucceed = jObj[ChunkFileHttp.CODE].ToString() == ChunkFileHttp.CODE_SUCCEED;
                    if (isSucceed)
                    {
                        RaiseUploading(readBytes, readProgress);
                    }
                    return this.isRunning && isSucceed;
                }
            }
        }


        private void RaiseUploading(long len, long progress)
        {
            var onUploadEvent = this.OnUploading;
            if (!onUploadEvent.IsNull())
            {
                onUploadEvent(this, new ChunkUploadProgressEventArgs(this.FileItem, len, progress));
            }
        }

        public bool Concat(string fileChunks)
        {
            var request = PostRequest(ConcatUrl, fileChunks);
            using (var resposne = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resposne.GetResponseStream()))
                {
                    var uploadResult = sr.ReadToEnd();
                    Debug.WriteLine(uploadResult);
                    JObject jObj = JObject.Parse(uploadResult);
                    return jObj[ChunkFileHttp.CODE].ToString() == ChunkFileHttp.CODE_SUCCEED;
                }
            }
        }

        private bool isRunning = true;
        public void Stop()
        {
            this.isRunning = false;

        }
    }
}
