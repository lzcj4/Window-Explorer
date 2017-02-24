using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    [ServiceContract(Name ="TestServer",Namespace ="http://www.TestServer.com")]
    public interface ITestServer
    {
        [OperationContract(Name ="GetName")]
        string GetName();

        [OperationContract(Name = "GetAge")]
        int GetAge();

        [OperationContract(Name = "GetAgeByName")]
        int GetAge(string name);
    }
}
