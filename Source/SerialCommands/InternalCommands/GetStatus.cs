using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class GetStatus : SerialCommandBase
    {
        public const byte Address = 0x09;

        private readonly ICommandWriter _serialWriter;
        private readonly ICommandEncryptor _commandEncryptor;

        public override bool CanWrite => true;

        public GetStatus(ICommandWriter serialWriter, ICommandEncryptor commandEncryptor)
        {
            _serialWriter = serialWriter;
            _commandEncryptor = commandEncryptor;
        }

        public override void Execute(int address, byte[] data)
        {
            if (address == Address)
            {
                // Get the status of all the internal parameters. The parts are separated by '|' (0x7C) and the fist byte is the address of the parameter.
                byte pipe = (byte)'|';
                byte[] status =
                {
                    SetAddressLength.Address, (byte)_commandEncryptor.AddressLength, pipe,
                    SetAddressFactor.Address, (byte)_commandEncryptor.AddressFactor, pipe,
                    SetLog.Address, 0x00, pipe
                };
                _serialWriter.Write(Address, status);
            }
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            return _serialWriter.Write(Address, new byte[] { });
        }
    }
}
