namespace SerialCommands
{
    public delegate void CommandDelegate(int address, byte[] data);
    public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);
}
