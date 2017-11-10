using HttpFileUploader.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader.Tasks
{

    class ChunkConcatTask : TaskBase
    {
        public event EventHandler<FileUploadProgressEventArgs> OnUploading;

        private long ChunkLen
        {
            get { return GlobalConst.ChunkLen; }
        }

        private string FilePath { get; set; }
        public ChunkConcatTask(string filePath)
        {
            this.FilePath = filePath;
        }

        public override void Run()
        {
            IList<FileChunkItem> chunkList = ChunkFileTaskGroup.GetChunks(this.FilePath, this.ChunkLen);
            JArray jArray = new JArray(chunkList.Select(item => item.ToJson()));
            JObject job = new JObject();
            job[ChunkFileHttp.FOLDERNAME] = chunkList[0].DestFolderName;
            job[ChunkFileHttp.FILENAME] = chunkList[0].FileName;
            job[ChunkFileHttp.FILE_INFOS] = jArray;
            ChunkFileHttp client = HttpFactory.Instance.GetChunkHttp();
            //this.IsCompleted = client.Concat(job.ToString());
            FileChannel.Instance.Send(job.ToString());
        }

        private void RaiseOnUploading(string filePath, long fileSize, long progress)
        {
            var uploadEvent = this.OnUploading;
            if (!uploadEvent.IsNull())
            {
                uploadEvent(this, new FileUploadProgressEventArgs(filePath, fileSize, progress));
            }
        }
    }

}