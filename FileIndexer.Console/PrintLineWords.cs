using FileIndexer.ConsoleHelpers;
using FileIndexer.Index;

namespace FileIndexer.Console
{
    public class PrintLineWords : ICommand
    {
        private const int MaxOutputLength = 1024;
        private readonly int _line;
        private readonly int[] _wordIndexes;
        private readonly LineIndex _lineIndex;
        private readonly IStringsSource _stringSource;

        public PrintLineWords(int line, int[] wordIndexes, LineIndex lineIndex, IStringsSource stringSource)
        {
            _line = line;
            _wordIndexes = wordIndexes;
            _lineIndex = lineIndex;
            _stringSource = stringSource;
        }

        public int[] WordIndexes
        {
            get { return _wordIndexes; }
        }

        public int Line
        {
            get { return _line; }
        }

        public void Execute()
        {
            try
            {
                if (_wordIndexes.Length == 0)
                {
                    var lineRange = _lineIndex.GetLineRange(_line);
                    PrintText(string.Empty, lineRange, _stringSource);
                }
                else
                {
                    for (int i = 0; i < _wordIndexes.Length; i++)
                    {
                        var wordRange = _lineIndex.GetWordRange(_line, _wordIndexes[i]);
                        var prefix = i == 0 ? string.Empty : " ";
                        PrintText(prefix, wordRange, _stringSource);
                    }
                }
                System.Console.WriteLine();
            }
            catch (LineNotFoundException ex)
            {
                Print.PrintExceptionMessage(ex);
            }
            catch (WordNotFoundException ex)
            {
                Print.PrintExceptionMessage(ex);
            }
        }

        private void PrintText(string prefix, Range range, IStringsSource stringSource)
        {
            var suffix = string.Empty;
            if (range.Length > MaxOutputLength)
            {
                range = range.Truncate(MaxOutputLength);
                suffix = "...";
            }

            var @string = range.IsEmpty ? string.Empty : stringSource.ReadString(range.Start, range.End);
            System.Console.Write("{0}{1}{2}", prefix, @string, suffix);
        }
    }
}