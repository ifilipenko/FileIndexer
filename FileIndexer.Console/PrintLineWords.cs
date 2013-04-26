﻿using System;
using FileIndexer.Index;

namespace FileIndexer.Console
{
    public class PrintLineWords : ICommand
    {
        private const int MaxOutputLength = 1024;
        private readonly int _lineIndex;
        private readonly int[] _wordIndexes;

        public PrintLineWords(int lineIndex, int[] wordIndexes)
        {
            _lineIndex = lineIndex;
            _wordIndexes = wordIndexes;
        }

        public int[] WordIndexes
        {
            get { return _wordIndexes; }
        }

        public int LineIndex
        {
            get { return _lineIndex; }
        }

        public void Execute(LineIndex index, IStringsSource stringSource)
        {
            try
            {
                if (_wordIndexes.Length == 0)
                {
                    var lineRange = index.GetLineRange(_lineIndex);
                    PrintText(string.Empty, lineRange, stringSource);
                }
                else
                {
                    for (int i = 0; i < _wordIndexes.Length; i++)
                    {
                        var wordRange = index.GetWordRange(_lineIndex, _wordIndexes[i]);
                        var prefix = i == 0 ? string.Empty : " ";
                        PrintText(prefix, wordRange, stringSource);
                    }
                }
                System.Console.WriteLine();
            }
            catch (LineNotFoundException ex)
            {
                PrintException(ex);
            }
            catch (WordNotFoundException ex)
            {
                PrintException(ex);
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

        private static void PrintException(Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(ex.Message);
            System.Console.ResetColor();
        }
    }
}