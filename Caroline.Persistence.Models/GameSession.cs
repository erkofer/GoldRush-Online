using Caroline.Persistence.Redis;
using StackExchange.Redis;

namespace Caroline.Persistence.Models
{
    public partial class GameSession : IIdentifiableEntity<RedisKey>
    {
        public GameSession(GameSessionEndpoint endpoint)
        {
            EndPoint = endpoint;
        }

        public GameSessionEndpoint EndPoint { get; set; }
        
        public long GameId { get; set; }

        public RedisKey Id
        {
            get { return GameSessionEndpoint.Serialize(EndPoint); }
            set { EndPoint = GameSessionEndpoint.Deserialize(value); }
        }
    }
}
