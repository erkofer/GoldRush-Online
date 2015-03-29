using System;
using System.Collections.Generic;

namespace Caroline.Persistence.Models
{
    public struct IpEndpoint
    {
        readonly string _localIp;
        readonly string _localPort;
        readonly string _remoteIp;
        readonly string _remotePort;
        public const char Delimiter = '-';

        public IpEndpoint(string localIp, string localPort, string remoteIp, string remotePort)
        {
            if (localIp == null) throw new ArgumentNullException("localIp");
            if (localPort == null) throw new ArgumentNullException("localPort");
            if (remoteIp == null) throw new ArgumentNullException("remoteIp");
            if (remotePort == null) throw new ArgumentNullException("remotePort");

            _localIp = localIp;
            _localPort = localPort;
            _remoteIp = remoteIp;
            _remotePort = remotePort;
        }

        public static IpEndpoint? TryParse(IDictionary<string, object> owinEnvironment)
        {
            if (owinEnvironment == null) throw new ArgumentNullException("owinEnvironment");

            var localIp = owinEnvironment["server.LocalIpAddress"] as string;
            var localPort = owinEnvironment["server.LocalPort"] as string;
            var remoteIp = owinEnvironment["server.RemoteIpAddress"] as string;
            var remotePort = owinEnvironment["server.RemotePort"] as string;
            if (localIp == null || localPort == null || remoteIp == null || remotePort == null)
                return null;

            return new IpEndpoint(localIp, localPort, remoteIp, remotePort);
        }

        public static bool TryParse(IDictionary<string, object> owinEnvironment, out IpEndpoint endpoint)
        {
            var result = TryParse(owinEnvironment);
            if (result != null)
            {
                endpoint = result.Value;
                return true;
            }

            endpoint = default (IpEndpoint);
            return false;
        }

        public static string Serialize(IpEndpoint ip)
        {
            //var ret = new byte[24];
            //BitConverter.GetBytes(ip._localIp).CopyTo(ret, 0);
            //BitConverter.GetBytes(ip._localPort).CopyTo(ret, 8);
            //BitConverter.GetBytes(ip._remoteIp).CopyTo(ret, 12);
            //BitConverter.GetBytes(ip._remotePort).CopyTo(ret, 20);
            //return ret;
            var sourceIp = ip._localIp;
            var sourcePort = ip._localPort;
            var destIp = ip._remoteIp;
            var destPort = ip._remotePort;

            var ret = new char[sourceIp.Length + sourcePort.Length + destIp.Length + destPort.Length + 3];
            var destIndex = 0;

            sourceIp.CopyTo(0, ret, destIndex, sourceIp.Length);
            destIndex += sourceIp.Length;
            ret[destIndex++] = Delimiter;

            sourcePort.CopyTo(0, ret, destIndex, sourcePort.Length);
            destIndex += sourcePort.Length;
            ret[destIndex++] = Delimiter;

            destIp.CopyTo(0, ret, destIndex, destIp.Length);
            destIndex += destIp.Length;
            ret[destIndex++] = Delimiter;

            destPort.CopyTo(0, ret, destIndex, destPort.Length);

            return new string(ret);
        }

        public static IpEndpoint Deserialize(string value)
        {
            var split = value.Split(Delimiter);
            if (split.Length != 4)
                throw new ArgumentException("value must be a colon-delimited list of integers of length 4.", "value");
            return new IpEndpoint(
                split[0],
                split[1],
                split[2],
                split[3]);
        }

        public string LocalIp
        {
            get { return _localIp; }
        }

        public string LocalPort
        {
            get { return _localPort; }
        }

        public string RemoteIp
        {
            get { return _remoteIp; }
        }

        public string RemotePort
        {
            get { return _remotePort; }
        }
    }
}
