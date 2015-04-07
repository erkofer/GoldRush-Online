using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class GameSession : IIdentifiableEntity<GameSessionEndpoint>
    {
        public GameSession(GameSessionEndpoint endpoint)
        {
            Id = endpoint;
        }

        public GameSessionEndpoint Id { get; set; }
    }
}
