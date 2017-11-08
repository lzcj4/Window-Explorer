using System;

namespace HttpFileUploader.Tasks
{
    public class ChunkUploadTask : TaskBase
    {
        public event EventHandler<ChunkUploadProgressEventArgs> OnUploading;

        public FileChunkItem ChunkItem { get; private set; }

        public ChunkUploadTask(FileChunkItem item)
        {
            if (item.IsNull())
            {
                throw new ArgumentNullException();
            }

            this.ChunkItem = item;
        }

        public override void Run()
        {
            ChunkFileHttp client = HttpFactory.Instance.GetChunkHttp();
            client.OnUploading += (sender, e) =>
            {
                RaiseUploading(e.ReadLen, e.Progress);
            };
            this.IsCompleted = client.Upload(ChunkItem);
        }


        private void RaiseUploading(long newReadLen, long progress)
        {
            var onUploadEvent = this.OnUploading;
            if (!onUploadEvent.IsNull())
            {
                onUploadEvent(this, new ChunkUploadProgressEventArgs(this.ChunkItem, newReadLen, progress));
            }
        }

        protected override void OnDisposing(bool isDisposed)
        {
            base.OnDisposing(isDisposed);
            if (!this.ChunkItem.IsNull())
            {
                this.ChunkItem.Dispose();
                this.ChunkItem = null;
            }
        }
    }
}
