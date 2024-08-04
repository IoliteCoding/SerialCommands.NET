using System;
using System.Runtime.Serialization;

namespace IoliteCoding.SerialCommands.Exceptions
{
    public class SerialCommandException : Exception
    {
        public SerialCommandException()
        {
        }

        public SerialCommandException(string message) : base(message)
        {
        }

        public SerialCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SerialCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
