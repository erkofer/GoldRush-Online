﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.RedisScripts;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis.Extensions
{
    static class CarolineScriptsRepoEx
    {
        public static async Task<RedisValue> StringGetSetExpiryAsync(this IDatabase db, CarolineScriptsRepo scripts, RedisKey key, RedisValue setValue, TimeSpan expiry)
        {
            return (RedisValue)await db.ScriptEvaluateAsync(scripts.StringGetSetExpiry, new[] { key }, new RedisValue[] { setValue, ((int)expiry.TotalMilliseconds).ToString(CultureInfo.InvariantCulture) });
        }

        public static async Task<long> IncrementExpiryAsync(this IDatabase db, CarolineScriptsRepo scripts, RedisKey key, RedisValue increment, TimeSpan expiry)
        {
            return (long)await db.ScriptEvaluateAsync(scripts.IncrementExpiry, new[] { key }, new RedisValue[] { increment, ((int)expiry.TotalMilliseconds).ToString(CultureInfo.InvariantCulture) });
        }

        public static void IncrementExpiryFaf(this IDatabase db, CarolineScriptsRepo scripts, RedisKey key, RedisValue increment, TimeSpan expiry)
        {
            db.ScriptEvaluate(scripts.IncrementExpiry, new[] { key }, new RedisValue[] { increment, ((int)expiry.TotalMilliseconds).ToString(CultureInfo.InvariantCulture) }, CommandFlags.FireAndForget);
        }

        public static async Task<TryLockResult> TryLock(this IDatabase db, CarolineScriptsRepo scripts, RedisKey key, TimeSpan expire)
        {
            var result = (RedisValue[])await db.ScriptEvaluateAsync(scripts.TryLock, new[] { key }, new RedisValue[] { (int)expire.TotalMilliseconds });
            return new TryLockResult((bool)result[0], TimeSpan.FromMilliseconds((int)result[1]));
        }
    }
}
