using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices.ComTypes;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Caroline.App
{
    public static class ProtoBufHelpers
    {
        // Make default values that will never be set to.
        private static readonly RuntimeTypeModel serializer;
        static ProtoBufHelpers()
        {
            serializer = TypeModel.Create();
            serializer.UseImplicitZeroDefaults = false;
        }

        public static string Serialize<T>(T data)
        {   
            // we have to serialize to an expandable memorystream then copy it to a byte[]
            // instead of serializing to a string directly
            var stream = new MemoryStream(4096);
            serializer.Serialize(stream, data);
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
