using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Models
{
    public class GameSessionEndpoint
    {
        public GameSessionEndpoint(IpEndpoint endpoint, long gameId)
        {
            EndPoint = endpoint;
            GameId = gameId;
        }

        public IpEndpoint EndPoint { get; set; }
        
        public long GameId { get; set; }

        public static string Serialize(GameSessionEndpoint session)
        {
            return IpEndpoint.Serialize(session.EndPoint) + ':' + session.GameId.ToStringInvariant();
        }

        public static GameSessionEndpoint Deserialize(string value)
        {
            var lastColonIndex = value.LastIndexOf(':');
            var ipAddress = value.Substring(0, lastColonIndex);
            var gameId = value.Substring(lastColonIndex + 1, value.Length);

            return new GameSessionEndpoint(IpEndpoint.Deserialize(ipAddress), long.Parse(gameId));
        }
    }
}
