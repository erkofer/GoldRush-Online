using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caroline.Persistence.Redis;
using JetBrains.Annotations;

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

    public partial class GameSessionWrapper : IIdentifiableEntity<GameSessionEndpoint>
    {
        public GameSessionWrapper(GameSessionEndpoint endpoint)
        {
            Id = endpoint;
        }

        public GameSessionEndpoint Id
        {
            get { return GameSession != null ? GameSession.Id : null; }
            set
            {
                if (value != null)
                {
                    if (GameSession == null)
                        GameSession = new GameSession(value);
                    else GameSession.Id = value;
                }
                else
                {
                    if (GameSession != null)
                        GameSession.Id = null;
                }
            }
        }
    }

    public partial class RateLimit
    {
        /// <summary>
        /// Checks if a request can be made under the current rate limit.
        /// </summary>
        /// <returns>If a request has been made under this rate limit.</returns>
        [Pure]
        public bool TryRequest()
        {
            if (InitialRequestDate == 0)
                InitialRequestDate = DateTime.UtcNow.ToBinary();
            var now = DateTime.UtcNow;
            if (now > DateTime.FromBinary(InitialRequestDate) + TimeSpan.FromSeconds(10))
            {
                // new ratelimit cycle
                InitialRequestDate = now.ToBinary();
                Requests = 1;
                return true;
            }

            // too early to restart rating, check to see if we can increment the existing rating
            if (Requests >= 15)
            {
                return false;
                
            } // rate limited
        

            Requests++;
            return true;
        }

        [Pure]
        static int GetUnixTime()
        {
            var dateTimeNow = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)dateTimeNow.TotalSeconds;
        }
    }
}
