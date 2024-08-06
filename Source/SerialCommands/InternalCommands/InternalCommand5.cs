﻿using IoliteCoding.SerialCommands.Abstraction;
using IoliteCoding.SerialCommands.Models;

namespace IoliteCoding.SerialCommands.InternalCommands
{
    internal class InternalCommand5 : SerialCommandBase
    {
        public const byte Address = 0x05;

        public override bool CanWrite => false;

        public override void Execute(int address, byte[] data)
        {
        }

        public override bool Write(ICommandWriter serialWriter, EncryptorOptions? encryptorOptions = null)
        {
            return false;
        }
    }
}
