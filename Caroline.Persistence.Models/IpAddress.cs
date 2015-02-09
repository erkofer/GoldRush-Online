using System;

namespace Caroline.Persistence.Models
{
    public class IPAddress
    {
        readonly long _sourceIp;
        readonly int _sourcePort;
        readonly long _destinationIp;
        readonly int _destinationPort;

        public IPAddress(long sourceIp, int sourcePort, long destinationIp, int destinationPort)
        {
            _sourceIp = sourceIp;
            _sourcePort = sourcePort;
            _destinationIp = destinationIp;
            _destinationPort = destinationPort;
        }

        public static byte[] Serialize(IPAddress ip)
        {
            var ret = new byte[24];
            BitConverter.GetBytes(ip._sourceIp).CopyTo(ret, 0);
            BitConverter.GetBytes(ip._sourcePort).CopyTo(ret, 8);
            BitConverter.GetBytes(ip._destinationIp).CopyTo(ret, 12);
            BitConverter.GetBytes(ip._destinationPort).CopyTo(ret, 20);
            return ret;
        }

        public static IPAddress Deserialize(byte[] value, int index)
        {
            if (index + 24 > value.Length)
                throw new ArgumentOutOfRangeException("index");
            return new IPAddress(
                BitConverter.ToInt64(value, index),
                BitConverter.ToInt32(value, index + 8),
                BitConverter.ToInt64(value, index + 12),
                BitConverter.ToInt32(value, index + 20));
        }

        public long SourceIp
        {
            get { return _sourceIp; }
        }

        public int SourcePort
        {
            get { return _sourcePort; }
        }

        public long DestinationIp
        {
            get { return _destinationIp; }
        }

        public int DestinationPort
        {
            get { return _destinationPort; }
        }
    }
}
