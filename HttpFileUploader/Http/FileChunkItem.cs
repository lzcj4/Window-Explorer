using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace HttpFileUploader
{
    public class FileChunkItem : IDisposable
    {
        public string DestFolderName { get; set; }
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
            this.FileName = fileName;
            this.DestFolderName = folderName;
            this.DestFileName = "{0}.tmp".StrFormat(chunkNo);
            this.ChunkLen = fileLen;
            this.ChunkCount = chunkCount;
            this.ChunkNo = chunkNo;

            this.Stream = stream;
        }

        public JObject ToJson()
        {
            JObject result = new JObject();
            result[ChunkFileHttp.FOLDERNAME] = this.DestFolderName;
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
}
