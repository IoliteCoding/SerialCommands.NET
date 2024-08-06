using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class DiscoverSettings : SerialCommandBase
    {
        private ICommandEncryptor _encryptor;

        public override bool CanWrite => true;

        public DiscoverSettings()
        {
        }

        public DiscoverSettings(ICommandEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        public override void Execute(int address, byte[] data)
        {
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            bool success = new SetAddressFactor(_encryptor).Write(serialWriter, EncryptorOptions.DefaultOptions);
            success &= new SetAddressLength(_encryptor).Write(serialWriter, EncryptorOptions.DefaultOptions);
            return success;
        }
    }
}
