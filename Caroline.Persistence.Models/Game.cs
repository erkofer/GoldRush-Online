using System.Globalization;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Models
{
    public partial class Game : IIdentifiableEntity<RedisKey>
    {
        public long Id { get; set; }

        RedisKey IIdentifiableEntity<RedisKey>.Id
        {
            get { return Id.ToStringInvariant(); }
            set { Id = long.Parse(value, CultureInfo.InvariantCulture); }
        }
    }
}