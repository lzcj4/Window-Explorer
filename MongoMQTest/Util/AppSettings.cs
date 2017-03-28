using System.Configuration;

namespace MongoMQTest
{
    public static class AppSettings
    {
        public const string Message_ConnectionString = "mongdb_message_connectionstring";

        public static int SeverCount
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ServerCount"]); }
        }

        public static string MessageConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[Message_ConnectionString].ConnectionString; }
        }
        
        public static long FileSize
        {
            get { return long.Parse(ConfigurationManager.AppSettings["FileSize"]); }
        }

        public static string TempFolder
        {
            get { return ConfigurationManager.AppSettings["TempFolder"]; }
        }

        public static string DestinationFolder
        {
            get { return ConfigurationManager.AppSettings["DestinationFolder"]; }
        }

    }
}
