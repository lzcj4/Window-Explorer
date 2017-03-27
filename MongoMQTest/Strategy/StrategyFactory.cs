using MongoMQTest.Queue;
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


        public IMsgStrategy<T> Get<T>() where T : MsgBase
        {
            //if (typeof(T) is AnalysisMsg)
            //{
            //    return new AnalysisMsgStrategy();
            //}
            throw new NotImplementedException();

        }

    }
}
