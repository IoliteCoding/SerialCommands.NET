using System;
using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.EventArguments
{
    public class CommandExecutionExceptionEventArgs : CommandReceivedEventArgs
    {
        public Exception Exception { get; }

        public CommandExecutionExceptionEventArgs(byte[] data, Exception exception) : base(-1, data, null)
        {
            Exception = exception;
        }

        public CommandExecutionExceptionEventArgs(int address, byte[] data, CommandDelegate command, Exception exception) : base(address, data, command)
        {
            Exception = exception;
        }

    }
}
