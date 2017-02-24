using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    public class TestServer : ITestServer
    {
        public int GetAge()
        {
            return 10;
        }

        public int GetAge(string name)
        {
            return 123;
        }

        public string GetName()
        {
            return "test";
        }

    }
}
