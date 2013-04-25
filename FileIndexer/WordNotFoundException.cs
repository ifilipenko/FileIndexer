using System;
using System.Runtime.Serialization;

namespace FileIndexer
{
    [Serializable]
    public class WordNotFoundException : Exception
    {
        public const string MessageText = "Word with index {0} is not found in line {1}";

        public WordNotFoundException(int lineNumber, int wordIndex) 
            : base(string.Format(MessageText, wordIndex, lineNumber))
        {
            LineNumber = lineNumber;
            WordIndex = wordIndex;
        }

        protected WordNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public int LineNumber { get; private set; }
        public int WordIndex { get; set; }
    }
}