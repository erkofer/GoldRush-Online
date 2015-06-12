using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class CarolineScriptsRepo : ReadOnlyTypeSafeDictionary<byte[]>
    {
        public static async Task<CarolineScriptsRepo> CreateAsync(IEnumerable<KeyValuePair<string, string>> scripts, IEnumerable<IServer> servers)
        {
            var scriptSha1 = new Dictionary<string, byte[]>();
            var serv = servers as IList<IServer> ?? servers.ToList();
            foreach (var pair in scripts)
            {
                for (int i = 0; i < serv.Count; i++)
                {
                    scriptSha1.Add(pair.Key, await serv[i].ScriptLoadAsync(pair.Value));
                }
            }
            return new CarolineScriptsRepo(scriptSha1);
        }

        public static CarolineScriptsRepo Create(IEnumerable<KeyValuePair<string, string>> scripts, IEnumerable<IServer> servers)
        {
            var scriptSha1 = new Dictionary<string, byte[]>();
            var serv = servers as IList<IServer> ?? servers.ToList();
            foreach (var pair in scripts)
            {
                for (int i = 0; i < serv.Count; i++)
                {
                    scriptSha1.Add(pair.Key, serv[i].ScriptLoad(pair.Value));
                }
            }
            return new CarolineScriptsRepo(scriptSha1);
        }
        
        CarolineScriptsRepo(IReadOnlyDictionary<string, byte[]> sha1)
            : base(sha1, '.')
        {
        }

        /// <summary>
        /// Return Value: 1 if rate limit exceeded, otherwise 0
        /// Key 1: A key to use as an expiring counter, e.g. "ratelimit:ip:127.0.0.1"
        /// Arg 1: Maximum number of calls
        /// Arg 2: Amount of time in ms
        /// </summary>
        public byte[] RateLimit { get; set; }
        public byte[] StringGetSetExpiry { get; set; }
        public byte[] IncrementExpiry { get; set; }
        public byte[] TryLock { get; set; }
        public byte[] PopMany { get; set; }
        public byte[] ZPush { get; set; }
    }
}
