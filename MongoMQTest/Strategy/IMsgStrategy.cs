using System.Collections.Generic;

namespace MongoMQTest
{
    public interface IMsgStrategy
    {
        IList<MsgBase> Split();
    }
}
