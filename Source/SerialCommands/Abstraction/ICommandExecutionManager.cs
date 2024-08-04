using System;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ICommandExecutionManager : IDisposable
    {
        int BufferCount { get; }

        bool TryAddCommandExecution(CommandExecutionContext execution);
    }
}
