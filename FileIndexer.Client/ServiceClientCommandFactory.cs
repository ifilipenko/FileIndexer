using FileIndexer.ConsoleHelpers;

namespace FileIndexer.Client
{
    internal class ServiceClientCommandFactory : ICommandFactory
    {
        private readonly string _serverAddress;

        public ServiceClientCommandFactory(string serverAddress)
        {
            _serverAddress = serverAddress;
        }

        public ICommand CreatePrintWordsCommand(int lineIndex, int[] wordIndexes)
        {
            return new GetFromServiceAndPrintLineWordsCommand(lineIndex, wordIndexes, _serverAddress);
        }

        public ICommand CreateExitCommand()
        {
            return new ExitCommand();
        }
    }
}