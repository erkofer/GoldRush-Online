using System;
using System.Web.Caching;
using Caroline.App.Models;

namespace Caroline.App
{
    public class GoldRushCache : IGoldRushCache
    {
        private readonly Cache _backingStore;

        public GoldRushCache(Cache backingStore)
        {
            _backingStore = backingStore;
        }

        public GameState GetGameData(string sessionGuid)
        {
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            return _backingStore.Get("game_" + sessionGuid) as GameState;
        }

        public void SetGameData(string sessionGuid, GameState state)
        {
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (state == null) throw new ArgumentNullException("state");

            _backingStore.Add("game_" + sessionGuid, state,
                dependencies: null, 
                absoluteExpiration: Cache.NoAbsoluteExpiration, 
                slidingExpiration: TimeSpan.FromSeconds(30), 
                priority: CacheItemPriority.Normal, 
                onRemoveCallback: null);
        }
    }
}
