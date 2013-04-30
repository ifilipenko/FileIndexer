using System;

namespace FileIndexer
{
    public class StringSymbolsFilter
    {
        private const char UnitSeparatorChar = (char) 0x001F;
        private const char DeleteChar = (char) 0x007F;
        public static readonly StringSymbolsFilter None = new StringSymbolsFilter(_ => false, ' ');

        public static StringSymbolsFilter ReplaceControlCharWith(char replaceCharacter)
        {
            return new StringSymbolsFilter(c => c < UnitSeparatorChar || c == DeleteChar, replaceCharacter);
        }

        private readonly Func<char, bool> _needReplace;
        private readonly char _replaceChar;

        private StringSymbolsFilter(Func<char, bool> needReplace, char replaceChar)
        {
            _needReplace = needReplace;
            _replaceChar = replaceChar;
        }

        public char ApplyFilter(char @char)
        {
            return _needReplace(@char) ? _replaceChar : @char;
        }
    }
}