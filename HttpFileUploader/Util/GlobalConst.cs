namespace HttpFileUploader.Util
{
    public static class GlobalConst
    {
        public const int ThreadCount = 4;
        public const int KB = 1024;
        public const int MB = KB * KB;
        public const int GB = KB * MB;

        /// <summary>
        /// Every chunk file length in bytes
        /// </summary>
        public const int ChunkLen = 100 * MB;
    }
}
