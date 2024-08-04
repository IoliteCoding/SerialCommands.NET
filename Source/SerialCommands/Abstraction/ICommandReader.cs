using System;
using System.IO;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ICommandReader
    {
        Stream Stream { get; }

        bool IsRunning { get; }

        Action<byte[]> MessageReceivedHandler { get; set; }

        Action<Exception> ExceptionHandler { get; set; }

        bool TryStart();

        bool TryStop();
    }
}