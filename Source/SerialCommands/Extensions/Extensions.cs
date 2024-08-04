using System;
using System.Collections.Generic;
using System.Linq;

namespace IoliteCoding.SerialCommands.Extensions
{
    public static class Extensions
    {

        public static IEnumerable<byte> ToBytes(this ulong value, int baseFactor) => value.ToBytes((byte)baseFactor);

        public static IEnumerable<byte> ToBytes(this ulong value, byte baseFactor)
        {
            if (value <= 0) yield break;

            if (value >= baseFactor)
            {
                foreach (var i in (value / baseFactor).ToBytes(baseFactor))
                    yield return i;
            }

            yield return (byte)(value % baseFactor);
        }


        public static ulong ToLong(this byte[] bytes, int baseFactor) => bytes.ToLong((byte)baseFactor);

        public static ulong ToLong(this byte[] bytes, byte baseFactor)
        {
            byte[] adr = bytes.Reverse().ToArray();

            ulong result = 0;
            for (int i = 0; i < adr.Length; i++)
                result += adr[i] * (ulong)Math.Pow(baseFactor, i);

            return result;
        }
    }
}
