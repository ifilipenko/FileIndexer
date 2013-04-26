using FileIndexer.ConsoleHelpers;
using FileIndexer.Index;

namespace FileIndexer.Console
{
    public class LocalIndexCommandFactory : ICommandFactory
    {
        private readonly LineIndex _lineIndex;
        private readonly IStringsSource _stringSource;

        public LocalIndexCommandFactory(LineIndex lineIndex, IStringsSource stringSource)
        {
            _lineIndex = lineIndex;
            _stringSource = stringSource;
        }

        public ICommand CreatePrintWordsCommand(int lineIndex, int[] wordIndexes)
        {
            return new PrintLineWords(lineIndex, wordIndexes, _lineIndex, _stringSource);
        }

        public ICommand CreateExitCommand()
        {
            return new ExitCommand();
        }
    }
}