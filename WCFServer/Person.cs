using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember(Name = "UserName")]
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
