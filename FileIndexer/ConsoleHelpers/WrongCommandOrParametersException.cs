using System;
using System.Runtime.Serialization;

namespace FileIndexer.ConsoleHelpers
{
    [Serializable]
    public class WrongCommandOrParametersException : Exception
    {
        public WrongCommandOrParametersException(string message) : base(message)
        {
        }

        public WrongCommandOrParametersException(string message, Exception inner) : base(message, inner)
        {
        }

        protected WrongCommandOrParametersException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}