using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.Models
{
    public class CommandExecutionContext
    {
        public int Address { set; get; }
        public byte[] Data { set; get; }
        public CommandDelegate Delegate { get; set; }

        public CommandExecutionContext(int address, byte[] data, CommandDelegate @delegate)
        {
            Address = address;
            Data = data;
            Delegate = @delegate;
        }
    }
}
