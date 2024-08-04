using IoliteCoding.SerialCommands.Abstraction;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class InternalCommand2 : SerialCommandBase
    {
        public const byte Address = 0x02;

        public override bool CanWrite => false;

        public override void Execute(int address, byte[] data)
        {
        }

        public override bool Write(ICommandWriter serialWriter)
        {
            return false;
        }
    }
}
