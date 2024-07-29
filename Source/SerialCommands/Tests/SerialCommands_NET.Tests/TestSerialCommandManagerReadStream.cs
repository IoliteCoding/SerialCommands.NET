using FluentAssertions;
using Moq;

namespace SerialCommands.Tests
{
    [TestClass]
    public class TestSerialCommandManagerReadStream
    {
        [TestMethod]
        public void Setting_stream_property_to_memoystream_and_writing_valid_command_should_trigger_CommandReceived_test()
        {
            // ASSIGN
            using ISerialCommandManager commandManager = new SerialCommandManager();

            bool testComleted = false;
            commandManager.CommandReceived += (s, e) =>
            {
                testComleted = true;
                e.Data.Should().BeEquivalentTo([0x55]);
            };

            using MemoryStream ms = new MemoryStream();
            commandManager.Stream = ms;
            byte[] message = { 0xFE, 0x02, 0x01, 0x55, 0xFF };

            // ACT
            ms.Write(message, 0, message.Length);
            ms.Flush();
            ms.Position = 0;

            DateTime startTime = DateTime.Now;
            while (!testComleted && startTime.AddSeconds(3) > DateTime.Now) { }

            // ASSERT
            ms.Position.Should().Be(message.Length);
            testComleted.Should().BeTrue();
            startTime.AddSeconds(3).Should().BeAfter(DateTime.Now);
        }

        [TestMethod]
        public void Setting_stream_property_to_memoystream_and_writing_valid_command_should_trigger_right_CommandReceived_test()
        {
            // ASSIGN
            using ISerialCommandManager commandManager = new SerialCommandManager();

            bool testComleted = false;
            commandManager.CommandReceived += (s, e) =>
            {                
                e.Address.Should().Be(0x01);
                e.Data.Should().BeEquivalentTo([0x55]);
            };

            var commandMock = new Mock<CommandDelegate>();
            commandMock.Setup(x => x(0x01, new byte[] { 0x55 })).Callback(() => testComleted = true);
            commandManager.AddCommand(1, commandMock.Object);

            using MemoryStream ms = new MemoryStream();
            commandManager.Stream = ms;
            byte[] message = { 0xFE, 0x02, 0x01, 0x55, 0xFF };

            // ACT
            ms.Write(message, 0, message.Length);
            ms.Flush();
            ms.Position = 0;

            DateTime startTime = DateTime.Now;
            while (!testComleted && startTime.AddSeconds(3) > DateTime.Now) { }

            // ASSERT
            ms.Position.Should().Be(message.Length);
            testComleted.Should().BeTrue();
            startTime.AddSeconds(3).Should().BeAfter(DateTime.Now);
            commandMock.Verify(x =>   x(0x01,new byte[] { 0x55 }),Times.Once());
        }
    }
}
