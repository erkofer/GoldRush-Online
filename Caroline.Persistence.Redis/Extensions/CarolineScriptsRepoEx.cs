using System;
using System.Globalization;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis.Extensions
{
    static class CarolineScriptsRepoEx
    {
        public static async Task<RedisValue> StringGetSetExpiryAsync(this IDatabase db,CarolineScriptsRepo scripts, RedisKey key, RedisValue setValue, TimeSpan expiry)
        {
            return (RedisValue)await db.ScriptEvaluateAsync(scripts.StringGetSetExpiry, new[] {key}, new RedisValue[] {setValue, ((int)expiry.TotalMilliseconds).ToString(CultureInfo.InvariantCulture)});
        }
    }
}
