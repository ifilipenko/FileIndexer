namespace FileIndexer
{
    public interface IIndexer
    {
        Range GetLineRange(int lineIndex);
        Range GetWordRange(int lineIndex, int wordIndex);
    }
}