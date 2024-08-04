using System;
using System.Runtime.Serialization;

namespace IoliteCoding.SerialCommands.Exceptions
{
    public class DecodingException : SerialCommandException
    {
        public byte[] MessageData { get; }

        private DecodingException()
        {
        }

        public DecodingException(string message, byte[] data) : base(message)
        {
            MessageData = data;
        }

        public DecodingException(string message, byte[] data, Exception innerException) : base(message, innerException)
        {
            MessageData = data;
        }

        protected DecodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
