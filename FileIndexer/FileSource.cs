using System.IO;
using System.Text;

namespace FileIndexer
{
    public class FileSource : IStringsSource
    {
        private readonly string _filePath;
        private readonly Encoding _encoding;

        public FileSource(string filePath, Encoding encoding)
        {
            _filePath = filePath;
            _encoding = encoding;
        }

        public string ReadString(long start, long end)
        {
            using (var stream = File.OpenRead(_filePath))
            using (var binaryReader = new BinaryReader(stream, _encoding))
            {
                binaryReader.BaseStream.Position = start;
                var chars = binaryReader.ReadChars((int) (end - start + 1));
                return new string(chars);
            }
        }
    }
}