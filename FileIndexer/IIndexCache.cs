namespace FileIndexer
{
    public interface IIndexCache
    {
        bool ActualCacheIsExists(string originalFilePath);
        LineIndex LoadFromCache(string originalFilePath);
        void Update(LineIndex lineIndex, string originalFilePath);
    }
}