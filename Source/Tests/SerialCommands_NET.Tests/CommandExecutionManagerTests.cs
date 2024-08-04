using FluentAssertions;
using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Delegates;
using IoliteCoding.SerialCommands.Models;
using Moq;

namespace IoliteCoding.SerialCommands
{
    [TestClass]
    public class CommandExecutionManagerTests
    {
        [TestMethod]
        public void ExecuteCommandTest()
        {
            int address = 1;

            CommandDelegate command = (add, data) =>
            {
                add.Should().Be(address);
                data.Should().BeEmpty();
            };

            CommandExecutionContext executionContext = new(address, [], command);

            using ICommandExecutionManager commandExecutionManager = new CommandExecutionManager();
            bool success = commandExecutionManager.TryAddCommandExecution(executionContext);
            success.Should().BeTrue();

        }

        [TestMethod]
        public void TryAddCommandExecution_Should_retrun_false_asfter_cancelation_test()
        {
            var commandExecutionContextMock = new Mock<CommandExecutionContext>(1, Array.Empty<byte>(), null);

            ICancellationTokenSourceManager tokenSource = new CancellationTokenSourceManager();
            ICommandExecutionManager commandExecutionManager = new CommandExecutionManager(tokenSource);

            tokenSource.Cancel();
            bool success = commandExecutionManager.TryAddCommandExecution(commandExecutionContextMock.Object);
            success.Should().BeFalse();
        }

        [TestMethod]
        public void TryAddCommandExecutionShouldThrowExceptionAfterDisposing()
        {
            int address = 1;

            CommandDelegate command = (address, data) =>
            {
                throw new NotSupportedException("This unit test should not execut this.");
            };

            CommandExecutionContext executionContext = new(address, [], command);

            ICommandExecutionManager commandExecutionManager = new CommandExecutionManager();
            using (commandExecutionManager)
            {

            }
            commandExecutionManager.Invoking(x => x.TryAddCommandExecution(executionContext)).Should().Throw<ObjectDisposedException>();

        }
    }
}
