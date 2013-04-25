using System;
using System.IO;
using System.Text;

namespace FileIndexer.Generator
{
    public class FileGenerator
    {
        private const int DefaultPartSizeForBigFiles = 10 * Volumes.Megabyte;
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);
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
                    GenerateBigFileWithOneBigWord(filePath);
                    break;
                case GeneratorMethod.BigFileWithOneLine:
                    GenerateBigFileWithOneLine(filePath);
                    break;
                case GeneratorMethod.BigFileWithManyLine:
                    GenerateBigFileWithManyLine(filePath);
                    break;
                case GeneratorMethod.EmptyFile:
                    File.WriteAllText(filePath, "");
                    break;
                case GeneratorMethod.ManyLinesWithoutWords:
                    File.WriteAllText(filePath, GenerateWhitespacesLines(100));
                    break;
            }
        }

        private void GenerateBigFileWithOneBigWord(string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
            using (var writer = new BinaryWriter(stream, FixedParameters.Encoding))
            {
                WriteBigWord(writer, 25 * Volumes.Gigabyte);
            }
        }

        private void GenerateBigFileWithOneLine(string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
            using (var writer = new BinaryWriter(stream, FixedParameters.Encoding))
            {
                WriteBigLine(writer, 25 * Volumes.Gigabyte);
            }
        }

        private void GenerateBigFileWithManyLine(string filePath)
        {
            using (var stream = File.OpenWrite(filePath))
            using (var writer = new BinaryWriter(stream, FixedParameters.Encoding))
            {
                WriteLine(writer, 512);
                WriteBigWord(writer, 10 * Volumes.Gigabyte);
                WriteLine(writer, 512);
                WriteBigLine(writer, 10 * Volumes.Gigabyte);
                WriteLine(writer, 512);
                WriteBigLine(writer, 5 * Volumes.Gigabyte);
            }
        }

        private void WriteBigLine(BinaryWriter writer, long size)
        {
            while (size > 0)
            {
                var partSize = (int)Math.Min(size, DefaultPartSizeForBigFiles);
                var wordCount = partSize < Volumes.Kilobyte ? 10 : 100;
                if (partSize < wordCount)
                {
                    wordCount = 1;
                }
                var linePart = GenerateRandomStringWithWords(wordCount, partSize);
                writer.Write(linePart);
                size -= linePart.Length;
            }
            writer.Write(Environment.NewLine);
        }

        private void WriteBigWord(BinaryWriter writer, long size)
        {
            while (size > 0)
            {
                var partSize = (int) Math.Min(size, DefaultPartSizeForBigFiles);
                var wordPart = GenerateRandomString(partSize, partSize);
                writer.Write(wordPart);
                size -= wordPart.Length;
            }
        }

        private void WriteLine(BinaryWriter writer, int stringSize)
        {
            writer.Write(GenerateRandomStringWithWords(3, stringSize) + Environment.NewLine);
        }

        private string GenerateRandomStringWithWords(int wordCount, int stringSize)
        {
            var maxWordSize = (int) (stringSize/(double) wordCount - wordCount);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < wordCount; i++)
            {
                var wordLength = _random.Next(100, maxWordSize + 1);
                var whiteSpacesLength = _random.Next(1, 5);
                stringBuilder.Append(GenerateRandomString(wordLength, wordLength));
                stringBuilder.Append(GenerateWhitespaceString(whiteSpacesLength));
            }
            if (stringBuilder.Length > stringSize)
            {
                stringBuilder.Length = stringSize;
            }
            else if (stringSize > stringBuilder.Length)
            {
                int trailLength = stringSize - stringBuilder.Length;
                stringBuilder.Append(GenerateRandomString(trailLength, trailLength));
            }
            return stringBuilder.ToString();
        }

        private string GenerateWhitespacesLines(int count)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                stringBuilder.AppendLine(GenerateWhitespaceString(100));
            }
            return stringBuilder.ToString();
        }

        private string GenerateWhitespaceString(int length)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var isSpace = (char) _random.Next(0, 2);
                stringBuilder.Append(isSpace == 0 ? '\t' : ' ');
            }
            return stringBuilder.ToString();
        }

        private string GenerateRandomString(int minLength, int maxLength)
        {
            var length = _random.Next(minLength, maxLength + 1);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var @char = (char) _random.Next('a', 'z');
                stringBuilder.Append(@char);
            }
            return stringBuilder.ToString();
        }
    }
}