using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Extensions;
using IoliteCoding.SerialCommands.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass]
    public class CommandEncryptor_Encoding_Tests
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
            // ASSIGN


            // ACT
            byte length = CommandEncryptor.GetAddressLength(address, factor);


            // ASSERT
            length.Should().Be((byte)addressLength);
        }

        [DataTestMethod]
        [DataRow(0UL)]
        [DataRow(1UL)]
        public void TestAddressLengthCalculatorShouldThrowException(ulong factor)
        {
            // ASSIGN


            // ACT
            var func = () => CommandEncryptor.GetAddressLength(1, factor);


            // ASSERT
            func.Should().Throw<ArgumentOutOfRangeException>();
        }

        [DataTestMethod]
        [DataRow(1, 1, new byte[] { 1, 2, 3 })]
        [DataRow(1, 112, new byte[] { 66, 27, 83, 214, 187, 253 })]
        [DataRow(2, 200, new byte[] { 49, 27, 168, 114 })]
        public void TestWithAddressLength(int addressLength, int address, byte[] data)
        {
            // ASSIGN
            var optionsMock = new Mock<IOptions<EncryptorOptions>>();
            optionsMock.SetupGet(x => x.Value).Returns(() =>
            {
                EncryptorOptions options = EncryptorOptions.DefaultOptions;
                options.AddressLength = addressLength;
                return options;
            });

            ICommandEncryptor encryptor = new CommandEncryptor(optionsMock.Object);


            byte[] addressBytes = ((ulong)address).ToBytes(encryptor.AddressFactor).ToArray();


            // ACT
            byte[] message = encryptor.Encode(address, data);


            // ASSERT
            message[0].Should().Be(encryptor.StartByte);
            message[1].Should().Be((byte)(addressLength + data.Length));

            for (int i = 0; i < addressLength; i++)
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
            // ASSIGN
            var optionsMock = new Mock<IOptions<EncryptorOptions>>();
            optionsMock.SetupGet(x => x.Value).Returns(() =>
            {
                EncryptorOptions options = EncryptorOptions.DefaultOptions;
                options.AddressLength = addressLength;
                return options;
            });

            ICommandEncryptor encryptor = new CommandEncryptor(optionsMock.Object);



            // ACT
            Action action = () => encryptor.Encode(address, []);


            // ASSERT
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

    }
}