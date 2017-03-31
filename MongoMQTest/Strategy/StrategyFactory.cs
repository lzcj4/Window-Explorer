using MongoMQTest.Model;
using System;

namespace MongoMQTest
{

    public class StrategyFactory
    {
        private static object lockObj = new object();
        private static StrategyFactory instance;
        public static StrategyFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        instance = new StrategyFactory();
                    }
                }
                return instance;
            }
        }


        public IMsgStrategy Get()
        {
            IMsgStrategy result;
            {
                result = new AnalysisMsgStrategy();
            }
            return result;
        }
    }
}
