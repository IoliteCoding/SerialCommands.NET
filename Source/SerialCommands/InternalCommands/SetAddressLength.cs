using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{

    internal class SetAddressLength : SerialCommandBase
    {
        public const byte Address = 0x00;

        private readonly ICommandEncryptor _encryptor;

        public SetAddressLength(ICommandEncryptor encryptor) : base()
        {
            _encryptor = encryptor;
        }

        public override bool CanWrite => true;

        public override void Execute(int address, byte[] data)
        {
            if (address == Address && data != null && data.Length > 0)
            {
                _encryptor.AddressLength = data[0];
            }
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            return serialWriter.Write(Address, new byte[] { (byte)_encryptor.AddressLength },encryptorOptions);
        }
    }
}
