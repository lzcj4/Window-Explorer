using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    public class PersonService : IPersonService
    {
        IList<Person> persons = new List<Person>
            {
              new Person() { Name = "AA", Id = 11, Address = "----AA" },
              new Person() { Name = "BB", Id = 22, Address = "----BB" },
                new Person() { Name = "CC", Id = 33, Address = "----CC" },
                new Person() { Name = "DD", Id = 44, Address = "----AA" },
            };

        public void Delete(string id)
        {
            Person p = persons.FirstOrDefault(item => item.Id == int.Parse(id));
            Console.WriteLine(string.Format("PersonService delete invoked:{0}", id));
        }

        public IList<Person> GetPerson()
        {
            Console.WriteLine(string.Format("PersonService GetPerson invoked"));
            return persons;
        }

        public Person GetPersonById(string id)
        {
            Person p = persons.FirstOrDefault(item => item.Id == int.Parse(id));
            Console.WriteLine(string.Format("PersonService GetPersonById invoked:{0}", id));
            return p;
        }
    }
}
