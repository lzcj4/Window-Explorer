using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                using (ServiceHost host = new ServiceHost(typeof(TestServer)))
                {
                    host.Opened += (sender, e) =>
                    {
                        Console.WriteLine("WCF test server launched");
                    };
                    host.Open();
                    Console.ReadLine();
                }
            });

            Task.Run(() =>
            {
                using (WebServiceHost host = new WebServiceHost(typeof(PersonService)))
                {
                    host.Opened += (sender, e) =>
                    {
                        Console.WriteLine("WCF person server launched");
                    };
                    host.Open();
                    Console.ReadLine();
                }
                //using (ServiceHost host = new ServiceHost(typeof(PersonService)))
                //{
                //    host.Opened += (sender, e) =>
                //    {
                //        Console.WriteLine("WCF person server launched");
                //    };
                //    host.Open();
                //    Console.ReadLine();
                //}
            });
            Console.ReadLine();
        }
    }
}
