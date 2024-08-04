using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass]
    public class CommandWriterTests
    {
        [DataTestMethod]
        [DataRow(1, 1, new byte[] { 0x00 })]
        [DataRow(101, 1, new byte[] { 0x00, 0x55, 0x14 })]
        [DataRow(1000, 2, new byte[] { 0x00, 0x27 })]
        [DataRow(1000, 3, new byte[] { 0x00, 0x27 })]
        public void WriteTest(int address, int addressLength, byte[] data)
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


            using MemoryStream ms = new MemoryStream();
            ICommandWriter commandWriter = new StreamManager(ms, encryptor);

            int messageLength = addressLength + data.Length;
            int totalMessageLength = messageLength + 3;


            //ACT
            commandWriter.Write(address, data);


            //ASSERT
            ms.Position.Should().Be(totalMessageLength);
            ms.Length.Should().Be(totalMessageLength);

            ms.Position = 0;
            ms.ReadByte().Should().Be(0xFE);
            ms.ReadByte().Should().Be((byte)messageLength);

            byte[] addressResult = new byte[addressLength];
            ms.Read(addressResult, 0, addressLength);
            addressResult.Should().BeEquivalentTo(encryptor.ToBaseFactor((ulong)address));

            foreach (byte b in data)
            {
                ms.ReadByte().Should().Be(b);
            }

            ms.ReadByte().Should().Be(0xFF);

        }
    }
}
