using System;
using System.Text;
using JetBrains.Annotations;

namespace Caroline.Persistence.Extensions
{
    static class ArrayEx
    {
        public static byte[] Combine([NotNull]params byte[][] arrays)
        {
            var totalLength = 0;
            for (var i = 0; i < arrays.Length; i++)
            {
                totalLength += arrays[i].Length;
            }
            var ret = new byte[totalLength];

            var offset = 0;
            for (var i = 0; i < arrays.Length; i++)
            {
                var array = arrays[i];
                Buffer.BlockCopy(array, 0, ret, offset, array.Length);
                offset += array.Length;
            }
            return ret;
        }

        public static byte[] Combine([NotNull]byte[] head, [NotNull] byte[] tail)
        {
            var rv = new byte[head.Length + tail.Length];
            Buffer.BlockCopy(head, 0, rv, 0, head.Length);
            Buffer.BlockCopy(tail, 0, rv, head.Length, tail.Length);
            return rv;
        }

        public static byte[] GetBytesNoEncoding(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetStringNoEncoding(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
