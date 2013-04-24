using System.IO;

namespace FileIndexer
{
    internal class FileGenerator
    {
        private readonly GeneratorMethod _generatorMethod;

        public FileGenerator(GeneratorMethod generatorMethod)
        {
            _generatorMethod = generatorMethod;
        }

        public void GenerateFile(string filePath)
        {
            switch (_generatorMethod)
            {
                case GeneratorMethod.BigFileWithOneBigWord:
                    break;
                case GeneratorMethod.BigFileWithOneLine:
                    break;
                case GeneratorMethod.BigFileWithManyLine:
                    break;
                case GeneratorMethod.EmptyFile:
                    File.WriteAllText(filePath, "");
                    break;
                case GeneratorMethod.ManyLinesWithoutWords:
                    File.WriteAllText(filePath, GenerateWhitespacesLines(100));
                    break;
            }
        }

        private string GenerateWhitespacesLines(int count)
        {
            return null;
        }
    }
}