using System.Collections.Generic;

namespace MongoMQTest
{
    public interface IMsgStrategy<T> where T : MsgBase
    {
        IList<T> Split();
    }
}
