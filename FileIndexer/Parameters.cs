namespace FileIndexer
{
    public class Parameters
    {
        public Parameters(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; private set; }
    }
}