namespace FileIndexer.ConsoleHelpers
{
    public interface ICommandFactory
    {
        ICommand CreatePrintWordsCommand(int lineIndex, int[] wordIndexes);
        ICommand CreateExitCommand();
    }
}