using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpFileUploader
{
    public class FileChunkItem
    {
        public string Name { get; set; }
        public int Num { get; set; }
        public long Len { get; set; }

        public FileChunkItem(string name, int num, long len)
        {
            this.Name = name;
            this.Num = num;
            this.Len = len;
        }
    }

    class UploadManager
    {
        private const int threadCount = 4;
        private const int KB = 1024;
        private const int MB = KB * KB;
        private const int GB = KB * MB;

        public bool Upload(string url, string filePath)
        {
            bool result = false;
            if (!filePath.IsFileExisted())
            {
                return result;
            }

            FileInfo fi = new FileInfo(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileFullName = Path.GetFileName(filePath);
            string ext = Path.GetExtension(filePath);
            long fileSize = fi.Length;
            long fileUctTime = fi.CreationTime.ToFileTimeUtc();
            string folderName = "{0}_{1}".StrFormat(fileName, fileUctTime);

            //100MB
            if (fileSize < 0.1 * GB)
            {
                //return UploadFile(url, filePath);
                ChunkFileStream stream = new ChunkFileStream(filePath, -1, fileSize);
                return UploadFile(url, folderName,new FileChunkItem(fileFullName,0,fileSize), stream);
            }
            else
            {
                IList<FileChunkItem> fileList = new List<FileChunkItem>();
                IList<Task<bool>> list = new List<Task<bool>>();
                long chunk = fileSize / threadCount;
                for (int i = 0; i < threadCount; i++)
                {
                    var j = i;
                    Task<bool> t = Task.Run<bool>(() =>
                    {
                        ChunkFileStream stream = null;
                        if (j == threadCount - 1)
                        {
                            long startOffset = j * chunk;
                            stream = new ChunkFileStream(filePath, startOffset, fileSize - startOffset);
                        }
                        else
                        {
                            stream = new ChunkFileStream(filePath, j * chunk, chunk);
                        }
                        var fileItem = new FileChunkItem(fileFullName, j, stream.Length);
                        fileList.Add(fileItem);
                        ChunkFileHttp client = new ChunkFileHttp();
                        return client.Upload(url, folderName, fileItem, stream);
                    });
                    list.Add(t);
                }

                bool[] resultList = Task.WhenAll<bool>(list).Result;
                if (resultList.All(item => { return item; }))
                {
                    FileMerge(folderName, fileFullName, fileList);
                }
                return true;
            }
        }

        private bool UploadFile(string url, string filePath)
        {
            ChunkFileHttp client = new ChunkFileHttp();
            return client.Upload(url, filePath);
        }

        private bool UploadFile(string url, string fileName, Stream stream)
        {
            ChunkFileHttp client = new ChunkFileHttp();
            return client.UploadEx(url, fileName, stream);
        }

        private bool UploadFile(string url, string folderName, FileChunkItem fileItem, Stream stream)
        {
            ChunkFileHttp client = new ChunkFileHttp();
            return client.Upload(url, folderName, fileItem, stream);
        }

        private bool FileMerge(string folderName, string fileName, IList<FileChunkItem> list)
        {
            JArray jArray = new JArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj["num"] = item.Num;
                obj["length"] = item.Len;
                jArray.Add(obj);
            }
            JObject job = new JObject();
            job["foldername"] = folderName;
            job["filename"] = fileName;
            job["files"] = jArray;
            ChunkFileHttp client = new ChunkFileHttp();
            return client.Merge("http://127.0.0.1:8000/file/merge/", job.ToString());
        }
    }
}
