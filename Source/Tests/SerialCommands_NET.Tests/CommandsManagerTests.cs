using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Delegates;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass()]
    public class CommandsManagerTests
    {
        [TestMethod()]
        public void AddCommandTest()
        {
            // ASSIGN
            using ISerialCommandManager manager = new CommandsManager(null, null, null, null, null);

            int internalCommandCount = manager.CommandCount;


            // ACT
            manager.AddCommand(10, (address, data) => { });
            int commandCount1 = manager.CommandCount;

            manager.AddCommand(11, new CommandDelegate(TestDelegate));
            int commandCount2 = manager.CommandCount;

            manager.AddCommand(11, TestDelegate);
            int commandCount3 = manager.CommandCount;


            // ASSERT
            commandCount1.Should().Be(internalCommandCount + 1);
            commandCount2.Should().Be(internalCommandCount + 2);
            commandCount3.Should().Be(internalCommandCount + 2);
        }

        [TestMethod()]
        public void RemoveCommandTest()
        {
            // ASSIGN
            using ISerialCommandManager manager = new CommandsManager(null, null, null, null, null);

            int internalCommandCount = manager.CommandCount;


            // ACT
            manager.AddCommand(10, (address, data) => { });
            manager.AddCommand(11, new CommandDelegate(TestDelegate));
            manager.AddCommand(12, TestDelegate);
            int commandCount1 = manager.CommandCount;

            manager.RemoveCommand(10);
            int commandCount2 = manager.CommandCount;

            manager.RemoveCommand(10);
            int commandCount3 = manager.CommandCount;

            // ASSERT
            commandCount1.Should().Be(internalCommandCount + 3);
            commandCount2.Should().Be(internalCommandCount + 2);
            commandCount3.Should().Be(internalCommandCount + 2);
        }

        [TestMethod()]
        public void ExecuteCommandTest()
        {
            // ASSIGN
            const int commandAddress = 0x0A;

            int commandReceivedAddress = -1;
            byte[] dataResult = null;
            var commandMock = new Mock<CommandDelegate>();
            commandMock.Setup(x => x(commandAddress, It.IsAny<byte[]>()))
                       .Callback<int, byte[]>((address, data) =>
                       {
                           commandReceivedAddress = address;
                           dataResult = data;
                       });

            CommandDelegate command = (address, data) => dataResult = data;

            ICommandEncryptor encryptor = new CommandEncryptor();
            using ISerialCommandManager manager = new CommandsManager(encryptor, null, null, null, null);

            manager.AddCommand(10, command);

            byte[] message = [0xFE, 0x02, commandAddress, 0x22, 0xFF];
            byte[] message2 = [0xFE, 0x03, 0x0F, 0x16, 0x44, 0xFF];


            // ACT
            bool success = manager.TryExecuteCommand(message);
            bool success2 = manager.TryExecuteCommand(message2);

            while (manager.BufferCount > 0) { }

            // ASSERT
            success.Should().BeTrue();
            success2.Should().BeFalse();
            dataResult.Should().NotBeNull();
            dataResult.Count().Should().Be(1);
            dataResult.Should().Equal([0x22]);
        }

        [TestMethod]
        public void Add_Remove_TryExecuteCommand_Should_Throw_Exception_After_Disposing_Test()
        {
            ICommandEncryptor encryptor = new CommandEncryptor();
            CommandsManager serialCommandManager = new CommandsManager(encryptor, null, null, null, null);
            using (serialCommandManager)
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
            var commandExecuterMock = new Mock<ICommandExecutionManager>();
            commandExecuterMock.Setup(x => x.Dispose()).Verifiable();

            ICommandEncryptor encryptor = encryptorMock.Object;
            ICommandExecutionManager executionManager = commandExecuterMock.Object;

            ISerialCommandManager commandManager = new CommandsManager(encryptor, null, null, executionManager, null);
            commandManager.Invoking(x => x.Dispose()).Should().NotThrow();

            commandExecuterMock.VerifyAll();
        }

        private void TestDelegate(int address, byte[] data)
        {

        }
    }
}
