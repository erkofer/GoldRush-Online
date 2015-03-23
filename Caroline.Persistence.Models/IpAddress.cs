using System;
using System.Globalization;
using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Models
{
    public struct IpAddress
    {
        readonly long _sourceIp;
        readonly int _sourcePort;
        readonly long _destinationIp;
        readonly int _destinationPort;

        public IpAddress(long sourceIp, int sourcePort, long destinationIp, int destinationPort)
        {
            _sourceIp = sourceIp;
            _sourcePort = sourcePort;
            _destinationIp = destinationIp;
            _destinationPort = destinationPort;
        }

        public static string Serialize(IpAddress ip)
        {
            //var ret = new byte[24];
            //BitConverter.GetBytes(ip._sourceIp).CopyTo(ret, 0);
            //BitConverter.GetBytes(ip._sourcePort).CopyTo(ret, 8);
            //BitConverter.GetBytes(ip._destinationIp).CopyTo(ret, 12);
            //BitConverter.GetBytes(ip._destinationPort).CopyTo(ret, 20);
            //return ret;
            var sourceIp = ip._sourceIp.ToStringInvariant();
            var sourcePort = ip._sourcePort.ToStringInvariant();
            var destIp = ip._destinationIp.ToStringInvariant();
            var destPort = ip._destinationPort.ToStringInvariant();

            var ret = new char[sourceIp.Length + sourcePort.Length + destIp.Length + destPort.Length + 3];
            var destIndex = 0;

            sourceIp.CopyTo(0, ret, destIndex, sourceIp.Length);
            destIndex += sourceIp.Length;
            ret[destIndex++] = ':';

            sourcePort.CopyTo(0, ret, destIndex, sourcePort.Length);
            destIndex += sourcePort.Length;
            ret[destIndex++] = ':';

            destIp.CopyTo(0, ret, destIndex, destIp.Length);
            destIndex += destIp.Length;
            ret[destIndex++] = ':';

            destPort.CopyTo(0, ret, destIndex, destPort.Length);

            return new string(ret);
        }

        public static IpAddress Deserialize(string value)
        {
            var split = value.Split(':');
            if (split.Length != 4)
                throw new ArgumentException("value must be a colon-delimited list of integers of length 4.", "value");
            return new IpAddress(
                long.Parse(split[0], CultureInfo.InvariantCulture),
                int.Parse(split[1], CultureInfo.InvariantCulture),
                long.Parse(split[2], CultureInfo.InvariantCulture),
                int.Parse(split[3], CultureInfo.InvariantCulture));
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
