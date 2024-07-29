using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SerialCommands.DependencyInjection
{
    public static class SerialCommandsExtensions
    {
        public static IServiceCollection AddSerialCommands(this IServiceCollection serviceCollection)
        {
            // serviceCollection.TryAddTransient<IManagerFactory, ManagerFactory>();
            //serviceCollection.TryAddSingleton(x => x.GetRequiredService<IManagerFactory>().CreateCommandManager());
            //serviceCollection.TryAddSingleton(x => x.GetRequiredService<IManagerFactory>().CreateCommandEncryptor());

            serviceCollection.TryAddSingleton<ICommandEncryptor, CommandEncryptor>();
            serviceCollection.TryAddSingleton<ICancellationTokenSourceManager, CancellationTokenSourceManager>();
            serviceCollection.TryAddSingleton<ICommandExecutionManager, CommandExecutionManager>();
            serviceCollection.TryAddSingleton<ISerialCommandManager, SerialCommandManager>();

            return serviceCollection;
        }
    }
}
