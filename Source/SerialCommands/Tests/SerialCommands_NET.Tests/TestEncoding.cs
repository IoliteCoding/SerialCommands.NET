using FluentAssertions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SerialCommands.NETTests
{
    [TestClass]
    public class TestEncoding
    {
        [DataTestMethod]
        [DataRow(1, 1UL, 200UL)]
        [DataRow(1, 199UL, 200UL)]
        [DataRow(2, 200UL, 200UL)]
        [DataRow(2, 39999UL, 200UL)]
        [DataRow(3, 40000UL, 200UL)]
        [DataRow(1, 99UL, 100UL)]
        [DataRow(2, 100UL, 100UL)]
        [DataRow(2, 9999UL, 100UL)]
        [DataRow(3, 10000UL, 100UL)]
        public void TestAddressLengthCalculator(int addressLength, ulong address, ulong factor)
        {
            byte length = CommandEncryptor.GetAddressLength(address, factor);

            length.Should().Be((byte)addressLength);
        }

        [DataTestMethod]
        [DataRow(0UL)]
        [DataRow(1UL)]
        public void TestAddressLengthCalculatorShouldThrowException(ulong factor)
        {
            var func = () => CommandEncryptor.GetAddressLength(1, factor);
            func.Should().Throw<ArgumentOutOfRangeException>();
        }

        [DataTestMethod]
        [DataRow(1, 1, new byte[] { 1, 2, 3 })]
        [DataRow(1, 112, new byte[] { 66, 27, 83, 214, 187, 253 })]
        [DataRow(2, 200,new byte[] { 49,27,168,114})]
        public void TestWithAddressLength(int addressLength, int address, byte[] data)
        {
            ICommandEncryptor encryptor = new CommandEncryptor(addressLength);

            byte[] message = encryptor.Encode(address, data);

            message[0].Should().Be(encryptor.StartByte);
            message[1].Should().Be((byte)(addressLength + data.Length));

            byte[] addressBytes = CommandEncryptor.ToBaseFactor((ulong)address, (ulong)encryptor.AddressFactor).ToArray();
            for (int i=0; i<addressLength; i++)
                message[2 + i].Should().Be(addressBytes[i]);

            for (int i = 0; i < data.Length; i++)
                message[2 + addressLength + i].Should().Be(data[i]);

            message.Last().Should().Be(encryptor.EndByte);
        }

        [DataTestMethod]
        [DataRow(1, 200)]
        [DataRow(2, 40000)]
        public void TestWithAddressLengthSouldThrowException(int addressLength, int address)
        {
            ICommandEncryptor encryptor = new CommandEncryptor(addressLength);
            encryptor.Invoking(x => x.Encode(address, Array.Empty<byte>())).Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}