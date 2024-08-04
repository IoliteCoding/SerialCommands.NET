using System;
using System.Runtime.Serialization;

namespace IoliteCoding.SerialCommands.Exceptions
{
    public class CommandExecutionException : SerialCommandException
    {
        public CommandExecutionException()
        {
        }

        public CommandExecutionException(string message) : base(message)
        {
        }

        public CommandExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
