using IoliteCoding.SerialCommands.EventArguments;

namespace IoliteCoding.SerialCommands.Delegates
{
    public delegate void CommandDelegate(int address, byte[] data);
    public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);
    public delegate void CommandExecutionExceptionThrownDelegate(object sender, CommandExecutionExceptionEventArgs e);
}
