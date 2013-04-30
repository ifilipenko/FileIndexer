using System;
using System.ServiceModel;
using System.Text;
using FileIndexer.ConsoleHelpers;
using FileIndexer.WCF.Binding;

namespace FileIndexer.Client
{
    internal class GetFromServiceAndPrintLineWordsCommand : ICommand
    {
        private readonly int _lineIndex;
        private readonly int[] _wordIndexes;
        private readonly string _serverAddress;
        private readonly EndpointAddress _endpointAddress;
        private readonly CustomTextEncodingHttpBinding _binding;

        public GetFromServiceAndPrintLineWordsCommand(int lineIndex, int[] wordIndexes, string serverAddress)
        {
            _lineIndex       = lineIndex;
            _wordIndexes     = wordIndexes;
            _serverAddress   = serverAddress;
            _endpointAddress = new EndpointAddress(new Uri(_serverAddress));
            _binding         = new CustomTextEncodingHttpBinding(Encoding.ASCII.EncodingName);
        }

        public void Execute()
        {
            try
            {
                using (var client = new IndexServiceClient(_binding, _endpointAddress))
                {
                    GetAndPrintLineWords(client);
                }
            }
            catch (Exception ex)
            {
                Print.PrintExceptionMessage(ex);
            }
        }

        private async void GetAndPrintLineWords(IndexServiceClient client)
        {
            try
            {
                var text = await client.GetAsync(_lineIndex, _wordIndexes);
                
                Console.WriteLine(text);
            }
            catch (FaultException ex)
            {
                Print.PrintExceptionMessage(ex);
            }
        }
    }
}