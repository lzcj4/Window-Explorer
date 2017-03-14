﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFClient.WCFServer {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.TestServer.com", ConfigurationName="WCFServer.TestServer")]
    public interface TestServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetName", ReplyAction="http://www.TestServer.com/TestServer/GetNameResponse")]
        string GetName();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetName", ReplyAction="http://www.TestServer.com/TestServer/GetNameResponse")]
        System.Threading.Tasks.Task<string> GetNameAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetAge", ReplyAction="http://www.TestServer.com/TestServer/GetAgeResponse")]
        int GetAge();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetAge", ReplyAction="http://www.TestServer.com/TestServer/GetAgeResponse")]
        System.Threading.Tasks.Task<int> GetAgeAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetAgeByName", ReplyAction="http://www.TestServer.com/TestServer/GetAgeByNameResponse")]
        int GetAgeByName(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.TestServer.com/TestServer/GetAgeByName", ReplyAction="http://www.TestServer.com/TestServer/GetAgeByNameResponse")]
        System.Threading.Tasks.Task<int> GetAgeByNameAsync(string name);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TestServerChannel : WCFClient.WCFServer.TestServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TestServerClient : System.ServiceModel.ClientBase<WCFClient.WCFServer.TestServer>, WCFClient.WCFServer.TestServer {
        
        public TestServerClient() {
        }
        
        public TestServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TestServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TestServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TestServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetName() {
            return base.Channel.GetName();
        }
        
        public System.Threading.Tasks.Task<string> GetNameAsync() {
            return base.Channel.GetNameAsync();
        }
        
        public int GetAge() {
            return base.Channel.GetAge();
        }
        
        public System.Threading.Tasks.Task<int> GetAgeAsync() {
            return base.Channel.GetAgeAsync();
        }
        
        public int GetAgeByName(string name) {
            return base.Channel.GetAgeByName(name);
        }
        
        public System.Threading.Tasks.Task<int> GetAgeByNameAsync(string name) {
            return base.Channel.GetAgeByNameAsync(name);
        }
    }
}