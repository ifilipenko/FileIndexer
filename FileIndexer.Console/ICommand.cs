using FileIndexer.Index;

namespace FileIndexer.Console
{
    public interface ICommand
    {
        void Execute(LineIndex index, IStringsSource stringSource);
    }
}