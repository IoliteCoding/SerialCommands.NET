using Microsoft.Extensions.DependencyInjection;

namespace SerialCommands.DependencyInjection
{
    public interface IManagerFactory
    {
        ICommandEncryptor CreateCommandEncryptor();
        ISerialCommandManager CreateCommandManager();
        ISerialCommandManager CreateCommandManager(CancellationTokenSource cancellationTokenSource);
        ICommandExecutionManager CreateCommandExecutionManager(CancellationTokenSource cancellationTokenSource);
    }

    public class ManagerFactory : IManagerFactory
    {
        public IServiceProvider ServiceProvider { get; }

        public ManagerFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public virtual ICommandEncryptor CreateCommandEncryptor()
        {
            return new CommandEncryptor();
        }

        public ICommandExecutionManager CreateCommandExecutionManager(CancellationTokenSource cancellationTokenSource)
        {
            return new CommandExecutionManager(cancellationTokenSource.Token);
        }

        public virtual ISerialCommandManager CreateCommandManager()
        {
            return CreateCommandManager(new CancellationTokenSource());
        }

        public virtual ISerialCommandManager CreateCommandManager(CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource ??= new CancellationTokenSource();
            ICommandExecutionManager executionManager = CreateCommandExecutionManager(cancellationTokenSource);
            ICommandEncryptor commandEncryptor=ServiceProvider.GetService<ICommandEncryptor>()??CreateCommandEncryptor();
            return new SerialCommandManager(CreateCommandEncryptor(), executionManager, cancellationTokenSource);
        }
    }
}
