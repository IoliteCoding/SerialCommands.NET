using System;
using IoliteCoding.SerialCommands.Delegates;

namespace IoliteCoding.SerialCommands.EventArguments
{
    public class CommandReceivedEventArgs : EventArgs
    {
        public int Address { get; set; }

        public byte[] Data { get; set; }

        public CommandDelegate Command { get; set; }

        public bool Handeled { get; set; }

        public CommandReceivedEventArgs(int address, byte[] data, CommandDelegate command)
        {
            Address = address;
            Data = data;
            Command = command;
        }
    }
}
