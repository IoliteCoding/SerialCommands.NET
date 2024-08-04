using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    public abstract class SerialCommandBase : ISerialCommand
    {
        private readonly CommandDelegate _delegate;
        public CommandDelegate Delegate => _delegate;

        public abstract bool CanWrite { get; }

        protected SerialCommandBase()
        {
            _delegate = new CommandDelegate(Execute);
        }

        public abstract void Execute(int address, byte[] data);

        public abstract bool Write(ICommandWriter serialWriter);

    }
}
