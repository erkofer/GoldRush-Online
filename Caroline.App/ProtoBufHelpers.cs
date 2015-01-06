using System;
using System.IO;
using ProtoBuf;

namespace Caroline.App
{
    public static class ProtoBufHelpers
    {
        public static string Serialize<T>(T data)
        {
            // we have to serialize to an expandable memorystream then copy it to a byte[]
            // instead of serializing to a string directly
            var stream = new MemoryStream(4096);
            Serializer.Serialize(stream, data);
            var message = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Position);
            return message;
        }

        public static T Deserialize<T>(string data)
        {
            var bytes = Convert.FromBase64String(data);
            var stream = new MemoryStream(bytes, false);
            return Serializer.Deserialize<T>(stream);
        }
    }
}
