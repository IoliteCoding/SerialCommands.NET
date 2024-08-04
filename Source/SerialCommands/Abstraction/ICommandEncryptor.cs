using IoliteCoding.SerialCommands.Models;
using System.Collections.Generic;

namespace IoliteCoding.SerialCommands.Abstraction
{
    public interface ICommandEncryptor
    {
        byte StartByte { get; set; }
        byte EndByte { get; set; }
        int AddressLength { get; set; }
        int AddressFactor { get; set; }

        byte[] Encode(int address, params byte[] bytes);

        byte[] Encode(int address, byte[] bytes, EncryptorOptions encryptorOptions);

        /// <summary>
        /// Decode the full byte array. Start & stop bytes can be includes but are not mandatory.
        /// </summary>
        /// <param name="bytes">The array of <see cref="byte"/></param>
        /// <returns></returns>
        (int address, byte[] data) Decode(byte[] bytes);

        IEnumerable<byte> ToBaseFactor(ulong value);
        ulong FromBaseFactor(params byte[] bytes);
    }
}
