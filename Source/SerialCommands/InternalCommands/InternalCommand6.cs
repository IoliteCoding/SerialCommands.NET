using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class InternalCommand6 : SerialCommandBase
    {
        public const byte Address = 0x06;

        public override bool CanWrite => false;

        public override void Execute(int address, byte[] data)
        {
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            return false;
        }
    }
}
