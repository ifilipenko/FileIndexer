using FileIndexer.Index;

namespace FileIndexer.Server
{
    public interface IIndexHolder
    {
        LineIndex GetIndex();
    }
}