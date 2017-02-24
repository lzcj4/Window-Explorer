using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFClient.WCFServer;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Binding binding = new BasicHttpBinding("BasicHttpBinding_PersonService");
            //EndpointAddress addr = new EndpointAddress("http://127.0.0.1:9999/PersonService");
            //TestServerClient tsc = new TestServerClient(binding, addr);
            //int a = tsc.GetAge();
            using (TestServerClient client = new TestServerClient())
            {
                client.GetAgeAsync().ContinueWith((ti) =>
                {
                    Console.WriteLine(string.Format("/**** get age async:{0}  ****/", ti.Result));
                });
                int age = client.GetAge();
                string name = client.GetName();
                int age2 = client.GetAgeByName(name);

                Console.WriteLine(string.Format("get age :{0},name:{1} , ageByName:{2}", age, name, age2));
            }


            using (PersonServer.PersonServiceClient client = new PersonServer.PersonServiceClient())
            {
                var pl = client.GetPerson();
            }

            Console.ReadLine();
        }
    }
}
