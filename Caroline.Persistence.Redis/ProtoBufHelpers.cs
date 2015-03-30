using System;
using System.IO;
using ProtoBuf;

namespace Caroline.Persistence.Redis
{
    public static class ProtoBufHelpers
    {
        public static string SerializeToString<T>(T data)
        {
            // we have to serialize to an expandable memorystream then copy it to a byte[]
            // instead of serializing to a string directly
            int length;
            var serialized = SerializeToBytesFast(data, out length);
            return Convert.ToBase64String(serialized, 0, length);
        }

        /// <summary>
        /// Serializes to a byte array that may be too large.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="populatedLength">The length of the byte array that has not been left unpopulated.</param>
        public static byte[] SerializeToBytesFast<T>(T data, out int populatedLength)
        {
            var stream = new MemoryStream(4096);
            Serializer.Serialize(stream, data);
            // cast to int is (sort of) safe, as byte[].Length is an int itself
            populatedLength = (int)stream.Position;
            return stream.GetBuffer();
        }

        /// <summary>
        /// Serializes to a byte array that is exactly the minimum required size.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        public static byte[] SerializeToBytes<T>(T data)
        {
            int length;
            var serialized = SerializeToBytesFast(data, out length);
            Array.Resize(ref serialized, length);
            return serialized;
        }

        public static T Deserialize<T>(string data)
        {
            var bytes = Convert.FromBase64String(data);
            return Deserialize<T>(bytes);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return Deserialize<T>(data, 0, data.Length);
        }

        public static T Deserialize<T>(byte[] data, int index, int count)
        {
            var stream = new MemoryStream(data, index, count, false, false);
            return Serializer.Deserialize<T>(stream);
        }
    }
}
