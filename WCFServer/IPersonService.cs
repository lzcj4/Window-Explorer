using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    [ServiceContract(Name = "PersonService", Namespace = "http://www.TestServer.com")]
    public interface IPersonService
    {
        [OperationContract]
        [WebGet(UriTemplate = "xml/all", ResponseFormat  = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        IList<Person> GetPerson();

        [OperationContract]
        [WebGet(UriTemplate = "json/{id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        Person GetPersonById(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "{id}", BodyStyle = WebMessageBodyStyle.Wrapped)]
        void Delete(string id);
    }
}
