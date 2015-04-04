using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Models
{
    public class GameSessionEndpoint : IIdentifiableEntity<string>
    {
        public GameSessionEndpoint()
        {
            
        }

        public GameSessionEndpoint(IpEndpoint endpoint, long gameId)
        {
            EndPoint = endpoint;
            GameId = gameId;
        }

        public IpEndpoint EndPoint { get; set; }

        public long GameId { get; set; }

        string IIdentifiableEntity<string>.Id
        {
            get
            {
                return IpEndpoint.Serialize(EndPoint) + ';' + GameId.ToStringInvariant();
            }
            set
            {
                var lastColonIndex = value.LastIndexOf(';');
                var ipAddress = value.Substring(0, lastColonIndex);
                var gameId = value.Substring(lastColonIndex + 1);

                EndPoint = IpEndpoint.Deserialize(ipAddress);
                GameId = long.Parse(gameId);
            }
        }
    }
}
