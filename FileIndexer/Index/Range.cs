using System;
using Newtonsoft.Json;

namespace FileIndexer.Index
{
    public struct Range
    {
        private long _start;
        private long _end;
        private long? _length;

        public Range(long start, long end)
        {
            if (start > end)
                throw new ArgumentException("Range is invalid");

            _start  = start;
            _end    = end;
            _length = null;
        }

        public long Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public long End
        {
            get { return _end; }
            set { _end = value; }
        }

        [JsonIgnore]
        public long Length
        {
            get { return (long) (_length ?? (_length = _end - _start + 1)); }
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get { return _length == 0; }
        }

        public Range Truncate(long newLength)
        {
            if (newLength == 0)
                return new Range();
            if (newLength < 0)
                throw new ArgumentException("Range length can not be less then 0");

            return Length < newLength ? new Range(Start, End) : new Range(Start, Start + newLength - 1);
        }

        public bool Equals(Range other)
        {
            return _start == other._start && _end == other._end;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Range && Equals((Range) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_start.GetHashCode()*397) ^ _end.GetHashCode();
            }
        }
    }
}