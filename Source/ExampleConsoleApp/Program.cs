using IoliteCoding.SerialCommands.DependencyInjection;
using IoliteCoding.SerialCommands.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using IoliteCoding.SerialCommands.EventArguments;

/* ---------------------------------------------------------
 * 
 * Created by IoliteCoding
 * 
 * This example goes together with the "simple_example.ino" sketch 
 * in the SerialCommands library for Arduino.
 * In the simple_example, every command who's address is >9 will be send back.
 * This will be displayed in the console by the event handler "ProcessSerialCommandReceived".
 * 
 * --------------------------------------------------------- */

internal class Program
{
    static int totalCommandsReceived = 0;

    private static void Main(string[] args)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSerialCommands();
        serviceCollection.AddSerialPortManager("COM9", 115200);
        serviceCollection.AddEncryptorOptions(addressLength: 2);
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        using (ISerialCommandManager serialCommandManager = serviceProvider.GetService<ISerialCommandManager>())
        {
            serialCommandManager.AddCommand(9, ClientStatus);
            serialCommandManager.CommandReceived += ProcessSerialCommandReceived;
            serialCommandManager.CommandReader.TryStart();

            serialCommandManager.CommandWriter.Write(9, []);
            Console.ReadKey();

            int pressedKey = -1;
            while (pressedKey != 9)
            {

                pressedKey = PrintOptions();
                Console.WriteLine();
                ProcessKey(pressedKey, serialCommandManager);

            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();

        serviceProvider.GetService<ICancellationTokenSourceManager>()?.Cancel();

    }


    static void ProcessSerialCommandReceived(object sender, CommandReceivedEventArgs e)
    {
        Console.WriteLine();
        Console.WriteLine("Command received: {0}", e.Address);
        Console.WriteLine("Data: {0}", BytesToString(e.Data));
        Console.WriteLine("Total commands received: {0}", ++totalCommandsReceived);
        Console.WriteLine();
    }

    static void ClientStatus(int address, byte[] data)
    {
        Console.WriteLine("Client status:");
        Console.WriteLine("Data: {0}", BytesToString(data));

        foreach (byte[] group in Split(data, 0x7C))
        {
            switch (group[0])
            {
                case 0:
                    Console.WriteLine("AddressLenght= {0}", group[1]);
                    break;
                case 1:
                    Console.WriteLine("AddressFactor= {0}", group[1]);
                    break;
                case 2:
                    Console.WriteLine("Internal command 2", BytesToString(group.Skip(1)));
                    break;
                case 3:
                    Console.WriteLine("Internal command 3", BytesToString(group.Skip(1)));
                    break;
                case 4:
                    Console.WriteLine("Internal command 4", BytesToString(group.Skip(1)));
                    break;
                case 5:
                    Console.WriteLine("Internal command 5", BytesToString(group.Skip(1)));
                    break;
                case 6:
                    Console.WriteLine("Internal command 6", BytesToString(group.Skip(1)));
                    break;
                case 7:
                    Console.WriteLine("Internal command 7", BytesToString(group.Skip(1)));
                    break;
                case 8:
                    Console.WriteLine("Logging= {0}", group[1] == 0 ? "Off" : "On");
                    break;
                default:
                    Console.WriteLine("Unknown group: {0} with data: {1}", group[0], BytesToString(group.Skip(1)));
                    break;
            }
        }

        if (totalCommandsReceived == 1)
            Console.WriteLine("Press any key to continu.");

    }

    static int PrintOptions()
    {
        Console.Clear();
        Console.WriteLine("Options:");
        Console.WriteLine("1 - Get client status (command 9)");
        Console.WriteLine("2 - Blink LED 1 time (command 10)");
        Console.WriteLine("3 - Blink LED continuous (command 52)");
        Console.WriteLine("4 - Stop continuous blinking (command 52)");
        Console.WriteLine("8 - Log simple message (command 316)");
        Console.WriteLine("9 - Exit");

        return ParseOption(Console.ReadKey());
    }

    static int ParseOption(ConsoleKeyInfo option) => option.Key switch
    {
        ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
        ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
        ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
        ConsoleKey.D4 or ConsoleKey.NumPad4 => 4,
        ConsoleKey.D5 or ConsoleKey.NumPad5 => 5,
        ConsoleKey.D6 or ConsoleKey.NumPad6 => 6,
        ConsoleKey.D7 or ConsoleKey.NumPad7 => 7,
        ConsoleKey.D8 or ConsoleKey.NumPad8 => 8,
        ConsoleKey.D9 or ConsoleKey.NumPad9 => 9,
        _ => 0
    };

    static void ProcessKey(int pressedKey, ISerialCommandManager serialCommandManager)
    {
        switch (pressedKey)
        {
            case 1:
                serialCommandManager.CommandWriter.Write(9, []);
                //Console.WriteLine("Press any key to continu.");
                //Console.ReadKey();
                break;
            case 2:
                Console.Write("How long should the LED be ON (x 100ms): ");
                byte onTime = ReadByte();
                serialCommandManager.CommandWriter.Write(10, [onTime]);
                break;
            case 3:
                Console.Write("How long should the LED be ON (x 100ms): ");
                byte onTimeCont = ReadByte();
                Console.Write("How long should the LED be OFF (x 100ms): ");
                byte offTimeCont = ReadByte();
                serialCommandManager.CommandWriter.Write(52, [1, onTimeCont, offTimeCont]);
                break;
            case 4:
                serialCommandManager.CommandWriter.Write(52, [0]);
                break;
            case 8:
                Console.Write("Message: ");
                string message = Console.ReadLine();
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(message);
                serialCommandManager.CommandWriter.Write(316, bytes);
                break;
            case 9:
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }

    static IEnumerable<byte[]> Split(byte[] bytes, byte separator)
    {
        while (bytes.Length > 0)
        {
            yield return bytes.TakeWhile(b => b != separator).ToArray();
            bytes = bytes.SkipWhile(b => b != separator).Skip(1).ToArray();
        }
    }

    static byte ReadByte()
    {
        byte result;
        while (!byte.TryParse(Console.ReadLine(), out result))
        {
            Console.WriteLine("Invalid input. Please try again.");
        }
        return result;
    }

    static string BytesToString(IEnumerable<byte> bytes) => string.Join(" ", bytes.Select(x => $"0x{x:X2}"));
}
