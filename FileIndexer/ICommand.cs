namespace FileIndexer
{
    public interface ICommand
    {
        void Execute(LineIndex index, IStringsSource stringSource);
    }
}