using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IoliteCoding.SerialCommands.DependencyInjection
{
    public static class SerialCommandsExtensions
    {
        public static IServiceCollection AddSerialCommands(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ICommandEncryptor, CommandEncryptor>();
            serviceCollection.TryAddSingleton<ICancellationTokenSourceManager, CancellationTokenSourceManager>();
            serviceCollection.TryAddSingleton<ICommandExecutionManager, CommandExecutionManager>();
            serviceCollection.TryAddSingleton<ISerialCommandManager, CommandsManager>();

            return serviceCollection;
        }

        public static IServiceCollection AddStreamManager(this IServiceCollection serviceCollection, Stream stream)
        {
            serviceCollection.TryAddSingleton(x => ActivatorUtilities.CreateInstance<StreamManager>(x, stream));
            serviceCollection.TryAddSingleton<ICommandWriter>(x => x.GetRequiredService<StreamManager>());
            serviceCollection.TryAddSingleton<ICommandReader>(x => x.GetRequiredService<StreamManager>());
            return serviceCollection;
        }

        public static IServiceCollection AddSerialPortManager(this IServiceCollection serviceCollection, string portName, int baudrate)
        {
            serviceCollection.TryAddSingleton(x => ActivatorUtilities.CreateInstance<SerialPortManager>(x, portName, baudrate));
            serviceCollection.TryAddSingleton<ICommandWriter>(x => x.GetRequiredService<SerialPortManager>());
            serviceCollection.TryAddSingleton<ICommandReader>(x => x.GetRequiredService<SerialPortManager>());
            return serviceCollection;
        }

        public static IServiceCollection AddEncryptorOptions(this IServiceCollection serviceCollection, EncryptorOptions options)
        {
            serviceCollection.TryAddTransient(x => options);
            return serviceCollection;
        }

        public static IServiceCollection AddEncryptorOptions(this IServiceCollection serviceCollection,
                                                             int addressLength = 1, int addressFactor = 200,
                                                             byte startByte = 0xFE, byte endByte = 0xFF)
        {
            serviceCollection.TryAddTransient<IOptions<EncryptorOptions>>(x =>
                                              new OptionsWrapper<EncryptorOptions>(new EncryptorOptions()
                                              {
                                                  AddressLength = addressLength,
                                                  AddressFactor = addressFactor,
                                                  StartByte = startByte,
                                                  EndByte = endByte
                                              }));
            return serviceCollection;
        }
    }
}
