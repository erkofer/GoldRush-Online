using System;
using System.Web;
using System.Web.Caching;
using Caroline.App.Models;
using JetBrains.Annotations;

namespace Caroline.App
{
    class GoldRushCache
    {
        [CanBeNull]
        public GameState GetGameData(string sessionGuid)
        {
            return HttpRuntime.Cache.Get("game_" + sessionGuid) as GameState;
        }

        public void SetGameData([NotNull] string sessionGuid, [NotNull] GameState state)
        {
            if (sessionGuid == null) throw new ArgumentNullException("sessionGuid");
            if (state == null) throw new ArgumentNullException("state");
            
            HttpRuntime.Cache.Add("game_" + sessionGuid, state,
                dependencies: null, 
                absoluteExpiration: Cache.NoAbsoluteExpiration, 
                slidingExpiration: TimeSpan.FromSeconds(30), 
                priority: CacheItemPriority.Normal, 
                onRemoveCallback: null);
        }
    }
}
