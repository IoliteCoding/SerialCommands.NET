using System;

namespace SerialCommands
{
    public class CommandReceivedEventArgs : EventArgs
    {
        public int Address;
        public byte[] Data;
        public CommandDelegate Command;

        public CommandReceivedEventArgs(int address, byte[] data, CommandDelegate command)
        {
            Address = address;
            Data = data;
            Command = command;
        }
    }
}
