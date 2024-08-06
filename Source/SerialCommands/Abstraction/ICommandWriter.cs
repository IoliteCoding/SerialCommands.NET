using IoliteCoding.SerialCommands.Models;
using System;
using System.IO;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ICommandWriter
    {
        Stream Stream { get; }

        Action<Exception> ExceptionHandler { get; set; }

        bool Write(byte[] buffer, int offset, int count);

        bool Write(int address, byte[] bytes);

        bool Write(int address, byte[] bytes, EncryptorOptions encryptorOptions);
    }
}