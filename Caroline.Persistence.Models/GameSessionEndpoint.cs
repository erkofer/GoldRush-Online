using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Models
{
    public partial class GameSessionEndpoint : IIdentifiableEntity<string>
    {
        public GameSessionEndpoint(IpEndpoint endpoint, long gameId)
        {
            EndPoint = endpoint;
            GameId = gameId;
        }

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
