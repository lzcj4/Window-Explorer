using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpFileUploader
{
    class FileChunkItem
    {
        public string Name { get; set; }
        public long Len { get; set; }

        public FileChunkItem(string name, long len)
        {
            this.Name = name;
            this.Len = len;
        }
    }

    class UploadManager
    {
        private const int threadCount = 2;
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
            string ext = Path.GetExtension(filePath);
            long fileSize = fi.Length;

            //100MB
            if (fileSize > 0.1 * GB)
            {
                //return UploadFile(url, filePath);
                ChunkFileStream stream = new ChunkFileStream(filePath, 0, fileSize);
                return UploadFile(url, "{0}{1}".StrFormat(fileName, ext), stream);
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
                        lock (this)
                        {
                            if (j == threadCount - 1)
                            {
                                long startOffset = j * chunk;
                                stream = new ChunkFileStream(filePath, startOffset, fileSize - startOffset);
                            }
                            else
                            {
                                stream = new ChunkFileStream(filePath, j * chunk, chunk);
                            }
                        }

                        string newName = "{0}_{1}{2}".StrFormat(fileName, j, ext);
                        fileList.Add(new FileChunkItem(newName, stream.Length));
                        return UploadFile(url, newName, stream);
                    });
                    list.Add(t);
                }

                bool[] resultList = Task.WhenAll<bool>(list).Result;
                if (resultList.All(item => { return item; }))
                {
                    FileMerge(fileList);
                }
                return true;
            }
        }

        MyHttp client = new MyHttp();
        private bool UploadFile(string url, string filePath)
        {
            return client.Upload(url, filePath);
        }

        private bool UploadFile(string url, string fileName, Stream stream)
        {
            return client.UploadEx(url, fileName, stream);
        }
        private bool FileMerge(IList<FileChunkItem> list)
        {
            JArray jArray = new JArray();
            foreach (var item in list)
            {
                JObject obj = new JObject();
                obj["name"] = item.Name;
                obj["length"] = item.Len;
                jArray.Add(obj);
            }
            return client.Merge("http://127.0.0.1:8000/file/merge/", jArray.ToString());
        }
    }
}
