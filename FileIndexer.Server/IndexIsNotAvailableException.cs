using System;
using System.Runtime.Serialization;

namespace FileIndexer.Server
{
    [Serializable]
    public class IndexIsNotAvailableException : Exception
    {
        public IndexIsNotAvailableException()
            : base("Index is not available, wait 5 minutes and try again")
        {
        }

        protected IndexIsNotAvailableException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}