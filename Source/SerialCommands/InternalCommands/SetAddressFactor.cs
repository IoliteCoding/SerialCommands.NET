using IoliteCoding.SerialCommands.Abstraction;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class SetAddressFactor : SerialCommandBase
    {
        public const byte Address = 0x01;

        private readonly ICommandEncryptor _encryptor;

        public override bool CanWrite => true;

        public SetAddressFactor(ICommandEncryptor encryptor) : base()
        {
            _encryptor = encryptor;
        }

        public override void Execute(int address, byte[] data)
        {
            if (address == Address && data != null && data[0] > 0)
            {
                _encryptor.AddressFactor = data[0];
            }
        }

        public override bool Write(ICommandWriter serialWriter)
        {
            return serialWriter.Write(Address, new byte[] { (byte)_encryptor.AddressFactor });
        }
    }
}
