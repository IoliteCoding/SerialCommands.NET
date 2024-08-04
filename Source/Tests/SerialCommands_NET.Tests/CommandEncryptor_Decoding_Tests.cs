using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Exceptions;
using IoliteCoding.SerialCommands.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass]
    public class CommandEncryptor_Decoding_Tests
    {
        private const string ExpectedExceptionMessageAddressLength = $"The length of the address \"?\" is less than the address length \"?\".";
        private const string ExpectedExceptionMessageDataLength = $"The length of the data (address + data) \"?\" is not equal to the message length \"?\".";

        [DataTestMethod]
        [DataRow(1, 0, new byte[] { }, new byte[] { 0xFE, 0x01, 0x00, 0xFF })]
        [DataRow(1, 0, new byte[] { }, new byte[] { 0xFE, 0x01, 0x00 })]
        [DataRow(1, 0, new byte[] { }, new byte[] { 0x01, 0x00 })]
        [DataRow(2, 201, new byte[] { }, new byte[] { 0xFE, 0x02, 0x01, 0x01, 0xFF })]
        [DataRow(2, 201, new byte[] { }, new byte[] { 0xFE, 0x02, 0x01, 0x01 })]
        [DataRow(2, 201, new byte[] { }, new byte[] { 0x02, 0x01, 0x01 })]
        [DataRow(1, 0, new byte[] { 0xCC, 0xDD }, new byte[] { 0xFE, 0x03, 0x00, 0xCC, 0xDD, 0xFF })]
        [DataRow(1, 0, new byte[] { 0xCC, 0xDD }, new byte[] { 0xFE, 0x03, 0x00, 0xCC, 0xDD })]
        [DataRow(1, 0, new byte[] { 0xCC, 0xDD }, new byte[] { 0x03, 0x00, 0xCC, 0xDD })]
        [DataRow(2, 201, new byte[] { 0xCC, 0xDD }, new byte[] { 0xFE, 0x04, 0x01, 0x01, 0xCC, 0xDD, 0xFF })]
        [DataRow(2, 201, new byte[] { 0xCC, 0xDD }, new byte[] { 0xFE, 0x04, 0x01, 0x01, 0xCC, 0xDD })]
        [DataRow(2, 201, new byte[] { 0xCC, 0xDD }, new byte[] { 0x04, 0x01, 0x01, 0xCC, 0xDD })]
        public void CommandEncryptor_Decode_Test(int addressLength, int address, byte[] data, byte[] message)
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
            var result = encryptor.Decode(message);


            // ASSERT
            result.address.Should().Be(address);
            result.data.Should().Equal(data);
        }

        [DataTestMethod]
        [DataRow(1, new byte[] { 0xFE, 0x01, 0x00, 0x00, 0xFF }, ExpectedExceptionMessageDataLength)]
        [DataRow(1, new byte[] { 0xFE, 0x01, 0xFF }, ExpectedExceptionMessageDataLength)]
        [DataRow(2, new byte[] { 0xFE, 0x01, 0x00, 0x01, 0xFF }, ExpectedExceptionMessageDataLength)]
        [DataRow(2, new byte[] { 0xFE, 0x01, 0x00, 0xFF }, ExpectedExceptionMessageAddressLength)]
        public void CommandEncryptor_Decode_Should_Throw_Exception_Test(int addressLength, byte[] message, string exceptionMessage)
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
            var decodeFunc = () => encryptor.Decode(message);


            // ASSERT
            decodeFunc.Should().ThrowExactly<DecodingException>()
                               .WithMessage(exceptionMessage)
                               .And.MessageData.Should().Equal(message);
        }
    }
}