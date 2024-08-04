using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ISerialCommand
    {
        CommandDelegate Delegate { get; }

        bool CanWrite { get; }

        void Execute(int address, byte[] date);

        bool Write(ICommandWriter serialWriter);
    }
}
