namespace FileIndexer
{
    internal class FileSource : IStringsSource
    {
        private readonly string _filePath;

        public FileSource(string filePath)
        {
            _filePath = filePath;
        }

        public string ReadString(long start, long end)
        {
            throw new System.NotImplementedException();
        }
    }
}