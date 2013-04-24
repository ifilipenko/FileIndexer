using System.Collections.Generic;
using System.Linq;

namespace FileIndexer
{
    public class LineIndex
    {
        private IList<Line> _lines;

        public LineIndex()
        {
            _lines = new List<Line>(0);
        }

        public IEnumerable<Line> Lines
        {
            get { return _lines; }
            private set { _lines = (IList<Line>) value; }
        }

        public void Add(Line line)
        {
            _lines.Add(line);
        }

        private bool Equals(LineIndex other)
        {
            return Lines.SequenceEqual(other.Lines);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LineIndex) obj);
        }

        public override int GetHashCode()
        {
            return (_lines != null ? _lines.GetHashCode() : 0);
        }

        public Range GetLineRange(int lineIndex)
        {
            return _lines[lineIndex].Range;
        }

        public Range GetWordRange(int lineIndex, int wordIndex)
        {
            return _lines[lineIndex].GetWordRange(wordIndex);
        }
    }
}