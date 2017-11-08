using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using HttpFileUploader.Util;

namespace HttpFileUploader
{
    public class FileChunkItem : IDisposable
    {
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string DestFileName { get; set; }

        public int ChunkCount { get; set; }
        public int ChunkNo { get; set; }
        public long ChunkLen { get; set; }

        /// <summary>
        /// File chunk stream 
        /// </summary>
        public Stream Stream { get; set; }

        public FileChunkItem()
        {

        }

        public FileChunkItem(string folderName, string fileName,
            int chunkCount, int chunkNo, long fileLen, Stream stream)
        {
            if (folderName.IsNullOrEmpty() || fileName.IsNullOrEmpty() ||
                chunkCount < 0 || chunkNo < 0 || fileLen < 0 ||
                stream.IsNull())
            {
                throw new InvalidDataException();
            }
            this.FolderName = folderName;
            this.FileName = fileName;
            this.ChunkLen = fileLen;
            this.DestFileName = "{0}.tmp".StrFormat(chunkNo);
            this.ChunkCount = chunkCount;
            this.ChunkNo = chunkNo;

            this.Stream = stream;
        }

        public JObject ToJson()
        {
            JObject result = new JObject();
            result[ChunkFileHttp.FOLDERNAME] = this.FolderName;
            result[ChunkFileHttp.FILENAME] = this.FileName;
            result[ChunkFileHttp.CHUNK_LEN] = this.ChunkLen;
            result[ChunkFileHttp.CHUNK_COUNT] = this.ChunkCount;
            result[ChunkFileHttp.CHUNK_NO] = this.ChunkNo;

            return result;
        }

        public void Dispose()
        {
            if (!this.Stream.IsNull())
            {
                this.Stream.Close();
                this.Stream = null;
            }
        }
    }
    public class FileUploadProgressEventArgs : EventArgs
    {
        public string FilePath { get; set; }

        /// <summary>
        /// Current file length
        /// </summary>
        public long Len { get; set; }

        /// <summary>
        /// Total read bytes
        /// </summary>
        public long Progress { get; set; }

        public FileUploadProgressEventArgs(string filePath, long len, long progress)
        {
            if (filePath.IsNullOrEmpty() ||
                 len < 0 || progress < 0 || progress > len)
            {
                throw new ArgumentException();
            }
            this.FilePath = filePath;
            this.Len = len;
            this.Progress = progress;
        }
    }

    class UploadManager
    {
        public event EventHandler<FileUploadProgressEventArgs> OnUploading;

        public IDictionary<string, long> Query(string filePath)
        {
            ChunkFileHttp client = HttpFactory.Instance.GetChunkHttp();
            return client.List(filePath);
        }

        private bool CheckFileIsUpload(IDictionary<string, long> dict, string fileName, long fileSize)
        {
            bool result = dict.ContainsKey(fileName) && dict[fileName] == fileSize;
            return result;
        }

        private bool UploadFile(IDictionary<string, long> dict, FileChunkItem item)
        {
            if (this.CheckFileIsUpload(dict, item.DestFileName, item.ChunkLen))
            {
                return true;
            }
            ChunkFileHttp client = HttpFactory.Instance.GetChunkHttp();
            client.OnUploading += (sender, e) =>
            {
                lock (this)
                {
                    this.ReadLen += e.ReadLen;
                }
                RaiseOnUploading(this.ReadLen);
            };
            return client.Upload(item);
        }

        private string FilePath { get; set; }
        private long FileSize { get; set; }
        private long ReadLen { get; set; }

        private void RaiseOnUploading(long progress)
        {
            var uploadEvent = this.OnUploading;
            if (!uploadEvent.IsNull())
            {
                uploadEvent(this, new FileUploadProgressEventArgs(this.FilePath, this.FileSize, progress));
            }
        }

        public bool Upload(string filePath)
        {
            bool result = false;
            if (!filePath.IsFileExisted())
            {
                return result;
            }

            this.FilePath = filePath;
            FileInfo fi = new FileInfo(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileFullName = Path.GetFileName(filePath);
            string ext = Path.GetExtension(filePath);
            FileSize = fi.Length;
            long fileUctTime = fi.CreationTime.ToFileTimeUtc();
            string folderName = "{0}_{1}".StrFormat(fileName, fileUctTime);

            IDictionary<string, long> existedFileDict = Query(folderName);

            //100MB
            if (FileSize < 0.1 *GlobalConst.GB)
            {
                ChunkFileStream stream = new ChunkFileStream(filePath, 0, FileSize);
                return UploadFile(existedFileDict, 
                    new FileChunkItem(folderName, fileFullName, 1, 0, FileSize, stream) { DestFileName = fileFullName }
                );
            }
            else
            {
                IList<FileChunkItem> fileList = new List<FileChunkItem>();
                IList<Task<bool>> list = new List<Task<bool>>();
                int chunkCount = GlobalConst.ThreadCount;
                long chunkLen = FileSize / chunkCount;
                for (int i = 0; i < chunkCount; i++)
                {
                    ChunkFileStream stream = null;
                    if (i == chunkCount - 1)
                    {
                        long startOffset = i * chunkLen;
                        stream = new ChunkFileStream(filePath, startOffset, FileSize - startOffset);
                    }
                    else
                    {
                        stream = new ChunkFileStream(filePath, i * chunkLen, chunkLen);
                    }
                    var fileItem = new FileChunkItem(folderName, fileFullName, chunkCount, i, stream.Length, stream);
                    fileList.Add(fileItem);

                    Task<bool> t = Task.Run<bool>(() =>
                    {
                        return UploadFile(existedFileDict, fileItem);
                    });
                    list.Add(t);
                }

                bool[] resultList = Task.WhenAll<bool>(list).Result;
                foreach (var item in list)
                {
                    item.Dispose();
                }

                if (resultList.All(item => { return item; }))
                {
                    if (this.CheckFileIsUpload(existedFileDict, fileFullName, FileSize))
                    {
                        return true;
                    }
                    FileMerge(folderName, fileFullName, fileList);
                }
                return true;
            }
        }

        private bool FileMerge(string folderName, string fileName, IList<FileChunkItem> list)
        {
            JArray jArray = new JArray(list.Select(item => item.ToJson()));
            JObject job = new JObject();
            job[ChunkFileHttp.FOLDERNAME] = folderName;
            job[ChunkFileHttp.FILENAME] = fileName;
            job[ChunkFileHttp.FILE_INFOS] = jArray;
            ChunkFileHttp client = new ChunkFileHttp();
            return client.Concat(job.ToString());
        }
    }
}
