namespace HttpFileUploader
{
    public class HttpFactory
    {
        private static HttpFactory instance;
        public static HttpFactory Instance
        {
            get
            {
                instance = instance ?? new HttpFactory();
                return instance;
            }
        }


        private string webHost = "127.0.0.0:8000";

        public string WebHost
        {
            get { return webHost; }
            set { webHost = value; }
        }
        

        public ChunkFileHttp GetChunkHttp()
        {
            ChunkFileHttp result = new ChunkFileHttp(this.WebHost);
            return result;
        }
    }
}
