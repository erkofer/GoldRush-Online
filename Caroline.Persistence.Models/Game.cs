using System.Globalization;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Models
{
    public partial class Game : IIdentifiableEntity<long>, IIdentifiableEntity<byte[]>
    {
        public long Id { get; set; }

        byte[] IIdentifiableEntity<byte[]>.Id
        {
            // use RedisKey casting for 
            get { return (RedisKey)Id.ToStringInvariant(); }
            set { Id = long.Parse((RedisKey)value, CultureInfo.InvariantCulture); }
        }
    }
}