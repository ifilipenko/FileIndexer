using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace FileIndexer
{
    public class Line
    {
        private IList<Range> _words;

        private Line()
        {
            _words = new List<Range>(0);
        }

        public Line(long start)
            : this()
        {
            Start = start;
        }

        public Line(long start, long end)
            : this()
        {
            Start = start;
            End = end;
        }

        public long Start { get; set; }
        public long End { get; set; }

        public IEnumerable<Range> Words
        {
            get { return _words; }
            private set { _words = (IList<Range>) value; }
        }

        public Range GetWordRange(int wordIndex)
        {
            return _words[wordIndex];
        }

        public void AddWord(long start, long end)
        {
            var range = new Range(start, end);
            if (range.IsEmpty)
                return;

            Debug.Assert(range.Start >= Start, "Word of line can not have start pos less then start pos of line");
            _words.Add(range);
        }

        [JsonIgnore]
        public Range Range
        {
            get { return new Range(Start, End); }
        }

        private bool Equals(Line other)
        {
            return Start == other.Start && End == other.End && Words.SequenceEqual(other.Words);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_words != null ? _words.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Start.GetHashCode();
                hashCode = (hashCode*397) ^ End.GetHashCode();
                return hashCode;
            }
        }
    }
}