using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader
{
    class ShareReadFileApi
    {
        public static Stream OpenShareReadFile(string filePath)
        {
            SafeFileHandle destFileHandle = CreateFile(filePath, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            FileStream stream = new FileStream(destFileHandle, FileAccess.Read);
            return stream;
        }

        public static Stream OpenShareReadWriteFile(string filePath)
        {
            SafeFileHandle destFileHandle = CreateFile(filePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE | FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            FileStream stream = new FileStream(destFileHandle, FileAccess.ReadWrite);
            return stream;
        }

        public static Stream CreateShareReadWriteFile(string filePath)
        {
            SafeFileHandle destFileHandle = CreateFile(filePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE | FILE_SHARE_READ, IntPtr.Zero, CREATE_NEW, 0, IntPtr.Zero);
            FileStream stream = new FileStream(destFileHandle, FileAccess.ReadWrite);
            return stream;
        }

        public static Stream OpenShareWriteFile(string filePath)
        {
            SafeFileHandle destFileHandle = CreateFile(filePath, GENERIC_WRITE, FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            FileStream stream = new FileStream(destFileHandle, FileAccess.Write);
            return stream;
        }

        #region Interop


        private const short FILE_ATTRIBUTE_NORMAL = 0x80;
        private const short INVALID_HANDLE_VALUE = -1;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint CREATE_NEW = 1;
        private const uint CREATE_ALWAYS = 2;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        private const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr securityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        #endregion
    }
}
