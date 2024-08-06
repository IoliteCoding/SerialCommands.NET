using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class SetLog : SerialCommandBase
    {
        public const byte Address = 0x08;

        public override bool CanWrite => true;

        public bool Log { get; internal set; }

        public SetLog() : base() { }

        public override void Execute(int address, byte[] data)
        {
            if (address == Address && data != null && data.Length > 0)
            {
                Log = data[0] == 1;
            }
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            return serialWriter.Write(Address, new byte[] { (byte)(Log ? 0x01 : 0x00) });
        }
    }
}
