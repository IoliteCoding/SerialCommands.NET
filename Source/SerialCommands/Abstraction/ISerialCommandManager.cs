using System;
using System.IO;
using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ISerialCommandManager : IDisposable
    {
        event CommandReceivedDelegate CommandReceived;
        event CommandExecutionExceptionThrownDelegate CommandExecutionExceptionThrown;

        int CommandCount { get; }
        int BufferCount { get; }
        Stream Stream { get; }

        ICommandReader CommandReader { get; }
        ICommandWriter CommandWriter { get; }

        void AddCommand(int address, CommandDelegate command);
        void RemoveCommand(int address);
        bool TryExecuteCommand(byte[] data);
       // bool TryGetCommand(int v, out ISerialCommand setAddressLengthCommand);
    }
}
