using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Delegates;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass]
    public class StreamManagerTests
    {
        [TestMethod]
        public void Setting_stream_property_to_memoystream_and_writing_valid_command_should_trigger_CommandReceived_test()
        {
            // ASSIGN
            const int commandAddress = 0x0A;
            const int commandData = 0x55;

            ICommandEncryptor encryptor = new CommandEncryptor();

            using MemoryStream ms = new MemoryStream();
            ICommandReader commandReader = new StreamManager(ms, encryptor);

            using ISerialCommandManager commandManager = new CommandsManager(encryptor, commandReader, null, null, null);

            int eventInvocationCount = 0;
            bool testComleted = false;
            int commandReceivedAddress = -1;
            byte[] commandReceivedData = null;
            commandManager.CommandReceived += (s, e) =>
            {
                eventInvocationCount++;
                commandReceivedAddress = e.Address;
                commandReceivedData = e.Data;
                testComleted = true;
            };

            byte[] message = { 0xFE, 0x02, commandAddress, commandData, 0xFF };

            bool commandReaderStarted;

            // ACT
            commandReaderStarted = commandReader.TryStart();

            ms.Write(message, 0, message.Length);
            ms.Flush();
            ms.Position = 0;

            DateTime startTime = DateTime.Now;
            while (!testComleted && startTime.AddSeconds(3) > DateTime.Now) { }

            // ASSERT
            commandReaderStarted.Should().BeTrue();
            ms.Position.Should().Be(message.Length);
            testComleted.Should().BeTrue();
            eventInvocationCount.Should().Be(1);
            commandReceivedAddress.Should().Be(commandAddress);
            commandReceivedData.Should().NotBeNull().And.BeEquivalentTo([commandData]);
            startTime.AddSeconds(3).Should().BeAfter(DateTime.Now);
        }

        [TestMethod]
        public void Setting_stream_property_to_memoystream_and_writing_valid_command_should_trigger_Corresponding_CommandReceived_test()
        {
            // ASSIGN
            const int commandAddress = 0x0A;
            const int commandData = 0x55;

            ICommandEncryptor encryptor = new CommandEncryptor();

            using MemoryStream ms = new MemoryStream();
            ICommandReader commandReader = new StreamManager(ms, encryptor);

            using ISerialCommandManager commandManager = new CommandsManager(encryptor, commandReader, null, null, null);

            int eventInvocationCount = 0;
            int commandReceivedAddress = -1;
            byte[] commandReceivedData = null;
            commandManager.CommandReceived += (s, e) =>
            {
                commandReceivedAddress = e.Address;
                commandReceivedData = e.Data;
                eventInvocationCount++;
            };

            bool testComleted = false;
            var commandMock = new Mock<CommandDelegate>();
            commandMock.Setup(x => x(commandAddress, new byte[] { commandData })).Callback(() => testComleted = true);

            commandManager.AddCommand(10, commandMock.Object);

            byte[] message = { 0xFE, 0x02, commandAddress, commandData, 0xFF };

            bool commandReaderStarted;

            // ACT
            commandReaderStarted = commandReader.TryStart();

            ms.Write(message, 0, message.Length);
            ms.Flush();
            ms.Position = 0;

            DateTime startTime = DateTime.Now;
            while (!testComleted && startTime.AddSeconds(3) > DateTime.Now) { }

            // ASSERT
            commandReaderStarted.Should().BeTrue();
            ms.Position.Should().Be(message.Length);
            testComleted.Should().BeTrue();
            eventInvocationCount.Should().Be(1);
            commandReceivedAddress.Should().Be(commandAddress);
            commandReceivedData.Should().Equal([commandData]);
            startTime.AddSeconds(3).Should().BeAfter(DateTime.Now);
            commandMock.Verify(x => x(commandAddress, new byte[] { commandData }), Times.Once());
        }
    }
}
