using System;
using System.Collections.Generic;
using System.Linq;

namespace SerialCommands
{
    public interface ICommandEncryptor
    {
        byte StartByte { get; }
        byte EndByte { get; }
        int AddressLength { get; }
        int AddressFactor { get; }

        byte[] Encode(int address, params byte[] bytes);
        (int address, byte[] data) Decode(byte[] bytes);

        IEnumerable<byte> ToBaseFactor(ulong value);
        ulong FromBaseFactor(params byte[] bytes);
    }

    public class CommandEncryptor : ICommandEncryptor
    {

        public byte StartByte => 0xFE;

        public byte EndByte => 0xFF;

        private readonly int _addressLength = 1;
        public int AddressLength => _addressLength;

        private readonly int _addressFactor=200;
        public int AddressFactor =>_addressFactor;

        public CommandEncryptor(int addressLength, int addressFactor)
        {
            _addressLength = addressLength;
            _addressFactor = addressFactor;
        }

        public CommandEncryptor(int addressLength)  :this(addressLength, 200) {}

        public CommandEncryptor()        {        }

        public byte[] Encode(int address, params byte[] bytes)
        {
            byte[] addressBytes = ToBaseFactor((ulong)address).ToArray();
            if (addressBytes.Length > AddressLength)
                throw new ArgumentOutOfRangeException(nameof(address), $"The length of the address \"{address}\" cannot exceed the length of {AddressLength} in Base{AddressFactor}");

            byte messageLength = (byte)(addressBytes.Length + bytes.Length);
            if (messageLength >= Math.Min(StartByte, EndByte))
                throw new ArgumentOutOfRangeException(nameof(messageLength), $"The length of the message (address bytes and data) cannot exceed {Math.Min(StartByte, EndByte)}");

            byte[] result = new byte[messageLength + 3];

            int counter = 0;
            result[counter++] = StartByte;
            result[counter++] = messageLength;

            for (int i = 0; i < addressBytes.Length; i++)
                result[counter++] = addressBytes[i];

            for (int i = 0; i < bytes.Length; i++)
                result[counter++] = bytes[i];

            result[counter++] = EndByte;


            return result;
        }

        public (int address, byte[] data) Decode(byte[] data)
        {
            if (data[0] == StartByte) data = data.Skip(1).ToArray();

            int messageLength = data[0];
            data = data.Skip(1).ToArray();

            int address = (int)FromBaseFactor(data.Take(AddressLength).ToArray());
            data = data.Skip(AddressLength).Take(messageLength - AddressLength).ToArray();

            return (address, data);
        }


        public IEnumerable<byte> ToBaseFactor(ulong value) => ToBaseFactor(value, (ulong)AddressFactor);

        public ulong FromBaseFactor(params byte[] bytes) => FromBaseFactor(AddressFactor, bytes);


        public static IEnumerable<byte> ToBaseFactor(ulong value, ulong factor)
        {
            if (value <= 0) yield break;

            if (value >= factor)
            {
                foreach (var i in ToBaseFactor(value / factor, factor))
                    yield return i;
            }

            yield return (byte)(value % 200);
        }

        public static ulong FromBaseFactor(int factor, params byte[] addressBytes)
        {
            byte[] adr = addressBytes.Reverse().ToArray();

            ulong result = 0;
            for (int i = 0; i < adr.Length; i++)
                result += adr[i] * (ulong)Math.Pow(factor, i);

            return result;
        }

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
