using System;
using System.Collections.Generic;
using System.Linq;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Exceptions;
using IoliteCoding.SerialCommands.Extensions;
using IoliteCoding.SerialCommands.Models;
using Microsoft.Extensions.Options;

namespace IoliteCoding.SerialCommands
{

    public class CommandEncryptor : ICommandEncryptor
    {

        private readonly EncryptorOptions _options;

        public byte StartByte { get => _options.StartByte; set => _options.StartByte = value; }

        public byte EndByte { get => _options.EndByte; set => _options.EndByte = value; }

        public int AddressLength { get => _options.AddressLength; set => _options.AddressLength = value; }

        public int AddressFactor { get => _options.AddressFactor; set => _options.AddressFactor = value; }

        public CommandEncryptor(IOptions<EncryptorOptions> options) : this(options.Value) { }

        protected CommandEncryptor(EncryptorOptions options)
        {
            _options = options.Copy();
        }

        public CommandEncryptor() : this(EncryptorOptions.DefaultOptions) { }


        public byte[] Encode(int address, params byte[] bytes) => Encode(address, bytes, _options);

        public byte[] Encode(int address, byte[] bytes, EncryptorOptions encryptorOptions)
        {
            byte[] addressBytes = ToBaseFactor((ulong)address).ToArray();
            if (addressBytes.Length > encryptorOptions.AddressLength)
                throw new ArgumentOutOfRangeException(nameof(address), $"The length of the address \"{address}\" cannot exceed the length of {AddressLength} in Base{AddressFactor}");

            byte messageLength = (byte)(addressBytes.Length + bytes.Length);
            if (messageLength >= Math.Min(encryptorOptions.StartByte, encryptorOptions.EndByte))
                throw new ArgumentOutOfRangeException(nameof(messageLength), $"The length of the message (address bytes and data) cannot exceed {Math.Min(StartByte, EndByte)}");

            byte[] result = new byte[messageLength + 3];

            int counter = 0;
            result[counter++] = encryptorOptions.StartByte;
            result[counter++] = messageLength;

            for (int i = 0; i < addressBytes.Length; i++)
                result[counter++] = addressBytes[i];

            for (int i = 0; i < bytes.Length; i++)
                result[counter++] = bytes[i];

            result[counter++] = encryptorOptions.EndByte;


            return result;
        }


        /// <inheritdoc/>
        public (int address, byte[] data) Decode(byte[] data)
        {
            byte[] messageData = data.ToArray();
            // Strip the startbyte
            if (messageData[0] == StartByte)
            {
                messageData = messageData.Skip(1).ToArray();
            }

            // Strip the endbyte
            messageData = messageData.TakeWhile(x => x != EndByte).ToArray();

            // Take the byte indicating the message length
            int messageLength = messageData[0];

            // Strip the byte wilt hte message length
            messageData = messageData.Skip(1).ToArray();

            if (messageData.Length != messageLength)
            {
                throw new DecodingException($"The length of the data (address + data) \"{messageData.Length}\" is not equal to the message length \"{messageLength}\".", data);
            }

            if (messageData.Length < AddressLength)
            {
                throw new DecodingException($"The length of the address \"{messageData.Length}\" is less than the address length \"{AddressLength}\".", data);
            }

            byte[] addressBytes = messageData.Take(AddressLength).ToArray();
            int address = (int)FromBaseFactor(addressBytes);

            byte[] dataBytes = messageData.Skip(AddressLength).ToArray();

            return (address, dataBytes);
        }

        public IEnumerable<byte> ToBaseFactor(ulong value)
        {
            byte[] result = value.ToBytes(AddressFactor).ToArray();
            int counter = result.Length;
            while (counter < AddressLength)
            {
                yield return 0;
                counter++;
            }
            foreach (var item in result)
            {
                yield return item;
            }
        }


        public ulong FromBaseFactor(params byte[] bytes) => bytes.ToLong(AddressFactor);

        public static byte GetAddressLength(ulong value, ulong factor)
        {
            if (factor <= 1) throw new ArgumentOutOfRangeException(nameof(factor));

            byte result = 1;
            while ((value = value / factor) > 0)
                result++;
            return result;
        }

    }
}
