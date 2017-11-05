using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HttpFileUploader
{
    public class ChunkFileStream : Stream
    {
        public long ReadLen { get; set; }
        public Stream FileStream { get; private set; }

        public ChunkFileStream(string filePath, long offset, long len)
        {
            if (!filePath.IsFileExisted())
            {
                throw new InvalidOperationException();
            }
            var stream = ShareReadFileApi.OpenShareReadFile(filePath);
            Initial(stream, offset, len);
        }

        public ChunkFileStream(Stream stream, long offset, long len)
        {
            Initial(stream, offset, len);
        }

        private void Initial(Stream stream, long offset, long len)
        {
            if (stream.IsNull() || offset < 0 || len < 0)
            {
                throw new InvalidOperationException();
            }

            this.FileStream = stream;
            this.StartIndex = offset;
            this.ReadLen = len;

            this.FileStream.Seek(StartIndex, SeekOrigin.Begin);
        }

        public override bool CanRead
        {
            get
            {
                bool result = this.Position <= this.LastIndex;
                return result;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        public long StartIndex { get; private set; }

        private long LastIndex
        {
            get
            {
                return this.StartIndex + this.ReadLen - 1;
            }
        }
        public override long Length
        {
            get
            {
                return this.ReadLen;
            }
        }

        public override long Position
        {
            get
            {
                return this.FileStream.Position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        long m_readLen = 0;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Position + count >= this.LastIndex)
            {
                count = (int)(this.LastIndex + 1 - this.Position);
                count = count < 0 ? 0 : count;
            }
            int readLen = this.FileStream.Read(buffer, offset, count);
            m_readLen += readLen;
            Debug.WriteLine("Read :{0} bytes, thread:{1}".StrFormat(m_readLen,Thread.CurrentThread.ManagedThreadId));
            return readLen;
        }

        long m_beginreadLen = 0;
        long lastPos = 0;
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            IAsyncResult result = null;
            lastPos = this.FileStream.Position;
            if (this.Position + count >= this.LastIndex)
            {
                count = (int)(this.LastIndex + 1 - this.Position);
                count = count < 0 ? 0 : count;
            }
            result = this.FileStream.BeginRead(buffer, offset, count, callback, state);
            // Debug.WriteLine("Thread:{0}, current pos:{1},read len:{2}", Thread.CurrentThread.Name, this.Position, count);
            m_beginreadLen += count;
            //Debug.WriteLine("BeginRead :{0}".StrFormat(m_beginreadLen));
            return result;
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            long readLen = this.FileStream.Position - lastPos;
            //  Debug.WriteLine("Thread:{0}, EndRead,readLen:{1}".StrFormat(Thread.CurrentThread.Name, readLen));
            return this.FileStream.EndRead(asyncResult);
        }
        long m_readasyncLen = 0;
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (this.Position + count >= this.LastIndex)
            {
                count = (int)(this.LastIndex + 1 - this.Position);
                count = count < 0 ? 0 : count;
            }
            var result = this.FileStream.ReadAsync(buffer, offset, count, cancellationToken);
            m_readasyncLen += result.Result;
            // Debug.WriteLine("ReadAsync :{0}".StrFormat(m_readasyncLen));
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!this.FileStream.IsNull())
            {
                this.FileStream.Dispose();
                this.FileStream = null;
            }
        }
    }
}
