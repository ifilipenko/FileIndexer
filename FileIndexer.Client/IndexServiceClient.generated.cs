using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace FileIndexer.Client
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "IndexService")]
    public interface IndexService
    {

        [OperationContractAttribute(Action = "http://tempuri.org/IndexService/Get", ReplyAction = "http://tempuri.org/IndexService/GetResponse")]
        string Get(int lineIndex, int[] words);

        [OperationContractAttribute(Action = "http://tempuri.org/IndexService/Get", ReplyAction = "http://tempuri.org/IndexService/GetResponse")]
        Task<string> GetAsync(int lineIndex, int[] words);
    }

    [GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IndexServiceChannel : IndexService, IClientChannel
    {
    }

    [DebuggerStepThrough]
    [GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class IndexServiceClient : ClientBase<IndexService>, IndexService
    {
        public IndexServiceClient()
        {
        }

        public IndexServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public IndexServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public IndexServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public IndexServiceClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public string Get(int lineIndex, int[] words)
        {
            return Channel.Get(lineIndex, words);
        }

        public Task<string> GetAsync(int lineIndex, int[] words)
        {
            return Channel.GetAsync(lineIndex, words);
        }
    }
}
