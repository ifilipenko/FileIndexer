using System;
using System.Runtime.Serialization;

namespace FileIndexer.Index
{
    [Serializable]
    public class LineNotFoundException : Exception
    {
        public const string MessageText = "Line {0} is not found by index";

        public LineNotFoundException(int lineNumber) 
            : base(string.Format(MessageText, lineNumber))
        {
            LineNumber = lineNumber;
        }

        protected LineNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public int LineNumber { get; private set; }
    }
}