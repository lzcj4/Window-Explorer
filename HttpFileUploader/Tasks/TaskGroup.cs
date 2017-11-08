using HttpFileUploader.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace HttpFileUploader.Tasks
{
    public abstract class TaskGroup
    {
        public string Name { get; set; }
        public TaskPriority Priority { get; set; }

        /// <summary>
        /// Sub tasks
        /// One file divide into multi tasks
        /// </summary>
        public IList<ITask> Tasks { get; protected set; }

        /// <summary>
        /// Sub groups
        /// Such as multi-file in one task
        /// </summary>
        public IList<TaskGroup> Groups { get; protected set; }

        public IEnumerable<ITask> AllTasks
        {
            get
            {
                foreach (var item in Tasks)
                {
                    yield return item;
                }
                foreach (var group in Groups)
                {
                    foreach (var t in group.Tasks)
                    {
                        yield return t;
                    }
                }
            }
        }

        public TaskGroup()
        {
            this.Tasks = new List<ITask>();
            this.Groups = new List<TaskGroup>();
        }

        public abstract void Create();
    }


    public class ChunkFileTaskGroup : TaskGroup
    {
        public event EventHandler<FileUploadProgressEventArgs> OnUploading;

        public string FilePath { get; private set; }
        private IList<string> files = new List<string>();

        public ChunkFileTaskGroup(params string[] files) : base()
        {
            if (files.IsNullOrEmpty())
            {
                throw new ArgumentException();
            }
            this.files = new List<string>(files);
        }

        public override void Create()
        {
            if (this.files.IsNullOrEmpty())
            {
                return;
            }

            if (this.files.Count == 1)
            {
                this.FilePath = this.files[0];
                this.Tasks = CreateTasks(this.FilePath);
            }
            else
            {
                foreach (string item in this.files)
                {
                    ChunkFileTaskGroup group = new ChunkFileTaskGroup(item);
                    group.Create();
                    this.Groups.Add(group);
                }
            }
        }

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

        private long FileSize = 0;
        private IList<ITask> CreateTasks(string filePath)
        {
            IList<ITask> result = new List<ITask>();
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

            this.FileSize = fileSize;

            IDictionary<string, long> existedFileDict = Query(folderName);

            //100MB
            if (fileSize < GlobalConst.ChunkLen)
            {
                ChunkFileStream stream = new ChunkFileStream(filePath, 0, fileSize);
                if (!CheckFileIsUpload(existedFileDict, fileFullName, fileSize))
                {
                    FileChunkItem item = new FileChunkItem(folderName, fileFullName, 1, 0, fileSize, stream)
                    { DestFileName = fileFullName };
                    result.Add(new ChunkUploadTask(item) { Priority = this.Priority });
                }
            }
            else
            {
                int chunkCount = (int)Math.Ceiling(1.0 * fileSize / GlobalConst.ChunkLen);
                for (int i = 0; i < chunkCount; i++)
                {
                    ChunkFileStream stream = null;
                    if (i == chunkCount - 1)
                    {
                        long startOffset = i * chunkCount;
                        stream = new ChunkFileStream(filePath, startOffset, fileSize - startOffset);
                    }
                    else
                    {
                        stream = new ChunkFileStream(filePath, i * chunkCount, GlobalConst.ChunkLen);
                    }

                    var item = new FileChunkItem(folderName, fileFullName, chunkCount, i, stream.Length, stream);
                    if (!CheckFileIsUpload(existedFileDict, item.DestFileName, item.ChunkLen))
                    {
                        result.Add(new ChunkUploadTask(item) { Priority = this.Priority });
                    }
                }
            }

            return result;
        }

        private long ReadLen { get; set; }
        private ChunkUploadTask CreateTask(FileChunkItem item)
        {
            ChunkUploadTask task = new ChunkUploadTask(item) { Priority = this.Priority };
            task.OnUploading += (sender, e) =>
            {
                lock (this)
                {
                    this.ReadLen += e.ReadLen;
                }
                RaiseOnUploading(this.ReadLen);
            };
            return task;
        }

        private void RaiseOnUploading(long progress)
        {
            var uploadEvent = this.OnUploading;
            if (!uploadEvent.IsNull())
            {
                uploadEvent(this, new FileUploadProgressEventArgs(this.FilePath, this.FileSize, progress));
            }
        }
    }
}
