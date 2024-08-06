using IoliteCoding.SerialCommands.Delegates;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ISerialCommand
    {
        CommandDelegate Delegate { get; }

        bool CanWrite { get; }

        void Execute(int address, byte[] date);

        bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null);
    }
}
