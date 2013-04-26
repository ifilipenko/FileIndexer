using System;
using System.ServiceModel;
using FileIndexer.ConsoleHelpers;

namespace FileIndexer.Client
{
    internal class GetFromServiceAndPrintLineWordsCommand : ICommand
    {
        private readonly int _lineIndex;
        private readonly int[] _wordIndexes;
        private readonly string _serverAddress;
        private readonly EndpointAddress _endpointAddress;
        private readonly BasicHttpBinding _binding;

        public GetFromServiceAndPrintLineWordsCommand(int lineIndex, int[] wordIndexes, string serverAddress)
        {
            _lineIndex       = lineIndex;
            _wordIndexes     = wordIndexes;
            _serverAddress   = serverAddress;
            _endpointAddress = new EndpointAddress(new Uri(_serverAddress));
            _binding         = new BasicHttpBinding();
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