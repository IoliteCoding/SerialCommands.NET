// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using SerialCommands;
using SerialCommands.DependencyInjection;

Console.WriteLine("Hello, World!");

//ICommandEncryptor encryptor=new CommandEncryptor(3);
//ulong address = 456703;
//foreach (var i in encryptor.ToBaseFactor(address))
//    Console.Write($"{i} ");

//Console.WriteLine();

//Console.WriteLine(CommandEncryptor.GetAddressLength(address, (ulong)encryptor.AddressFactor));

//var message = encryptor.Encode((int)address, 25, 14);
//Console.WriteLine(string.Join(" ", message));

//var origin=encryptor.Decode(message);
//Console.WriteLine(origin.address.ToString());
//Console.WriteLine(string.Join(" ",  origin.data));

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddSerialCommands();
IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

using (ISerialCommandManager serialCommandManager = serviceProvider.GetService<ISerialCommandManager>())
{
    serialCommandManager.AddCommand(0, (x, y) => Console.WriteLine($"AddressLength: {y[0]}"));

    using MemoryStream memoryStream = new();
    serialCommandManager.Stream = memoryStream;

    memoryStream.Write(new byte[] { 0xFE, 0x02, 0x00, 0x03, 0xFF });
    memoryStream.Position = 0;

    Console.ReadKey();
}

Console.ReadKey();