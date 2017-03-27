using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoMQTest
{
    public static class AppSettings
    {
        public const string Message_ConnectionString = "mongdb_message_connectionstring";
        private const string Analysis_ConnectionString = "mongdb_tasks_connectionstring";

        public static int SeverCount
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ServerCount"]); }
        }

        public static string MessageConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[Message_ConnectionString].ConnectionString; }
        }

        public static string AnalysisConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[Analysis_ConnectionString].ConnectionString; }
        }

    }
}
