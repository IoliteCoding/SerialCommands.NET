using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SerialCommands;

namespace SerialCommands.Tests
{
    [TestClass()]
    public class TestSerialCommandsManager
    {
        [TestMethod()]
        public void AddCommandTest()
        {
            using ISerialCommandManager manager = new SerialCommandManager();
            manager.AddCommand(1, (address, data) => { });
            manager.CommandCount.Should().Be(1);

            manager.AddCommand(2, new CommandDelegate(TestDelegate));
            manager.CommandCount.Should().Be(2);

            manager.AddCommand(1, TestDelegate);
            manager.CommandCount.Should().Be(2);

        }

        [TestMethod()]
        public void RemoveCommandTest()
        {
            using ISerialCommandManager manager = new SerialCommandManager();

            manager.AddCommand(1, (address, data) => { });
            manager.AddCommand(2, new CommandDelegate(TestDelegate));
            manager.AddCommand(3, TestDelegate);
            manager.CommandCount.Should().Be(3);

            manager.RemoveCommand(1);
            manager.CommandCount.Should().Be(2);

            manager.RemoveCommand(1);
            manager.CommandCount.Should().Be(2);
        }

        [TestMethod()]
        public void ExecuteCommandTest()
        {
            CommandDelegate command = (int address, byte[] data) => data.Count().Should().Be(1);

          using  ISerialCommandManager manager = new SerialCommandManager();
            manager.AddCommand(1, command);

            byte[] message = [0xFE, 0x02, 0x01, 0x22, 0xFF];
            bool success = manager.TryExecuteCommand(message);
            success.Should().BeTrue();

            byte[] message2 = [0xFE, 0x03, 0x0A, 0x16, 0x44, 0xFF];
            success = manager.TryExecuteCommand(message2);
            success.Should().BeFalse();

                while (manager.BufferCount > 0) { }
        }

        [TestMethod]
        public void Add_Remove_TryExecuteCommand_Should_Throw_Exception_After_Disposing_Test()
        {
            SerialCommandManager serialCommandManager = new SerialCommandManager();
            using ( serialCommandManager)
            {
            }

            serialCommandManager.Invoking(x => x.AddCommand(1, TestDelegate)).Should().Throw<ObjectDisposedException>();
            serialCommandManager.Invoking(x => x.RemoveCommand(1)).Should().Throw<ObjectDisposedException>();
            serialCommandManager.Invoking(x => x.TryExecuteCommand([])).Should().Throw<ObjectDisposedException>();
        }

        [TestMethod]
        public void SerialCommandManagerShouldDisposeCommandExecuterOnDisposingTest()
        {
            var encryptorMock = new Mock<ICommandEncryptor>();
            var commandExecuterMock=new Mock<ICommandExecutionManager>();
            commandExecuterMock.Setup(x => x.Dispose()).Verifiable();

            ICommandEncryptor encryptor = encryptorMock.Object;
            ICommandExecutionManager executionManager = commandExecuterMock.Object;

            ISerialCommandManager commandManager =new SerialCommandManager(encryptor,executionManager,null);
            commandManager.Invoking(x=>x.Dispose()).Should().NotThrow();

            commandExecuterMock.VerifyAll();
        }

        private void TestDelegate(int address, byte[] data)
        {

        }
    }
}
