namespace IoliteCoding.SerialCommands.Models
{
    public class EncryptorOptions
    {
        public static EncryptorOptions DefaultOptions => new EncryptorOptions() { StartByte = 0xFE, EndByte = 0xFF, AddressLength = 1, AddressFactor = 200 };

        public byte StartByte { get; set; }
        public byte EndByte { get; set; }
        public int AddressLength { get; set; }
        public int AddressFactor { get; set; }

        public EncryptorOptions Copy()
        {
            return new EncryptorOptions()
            {
                StartByte = StartByte,
                EndByte = EndByte,
                AddressLength = AddressLength,
                AddressFactor = AddressFactor
            };
        }
    }
}
