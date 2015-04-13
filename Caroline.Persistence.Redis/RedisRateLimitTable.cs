using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisRateLimitTable<TId> : RedisEntityTableBase<TId>, IRateLimitTable<TId>
    {
        readonly IDatabase _db;
        readonly byte[] _rateLimitScript;
        readonly RedisValue[] _cachedAttemptArgs;

        public RedisRateLimitTable(IDatabaseArea db, ISerializer<TId> keySerializer, int maxRequests, TimeSpan duration)
            : base(keySerializer)
        {
            _db = db;
            _rateLimitScript = db.Scripts.RateLimit;
            _cachedAttemptArgs = new RedisValue[] { maxRequests, (int)Math.Ceiling(duration.TotalSeconds) };
        }

        public async Task<bool> Attempt(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            // in RedisResult bool casting, a "1" reply casts to true and "0" casts to false
            return (bool)await _db.ScriptEvaluateAsync(_rateLimitScript, new RedisKey[] { tid }, _cachedAttemptArgs);
        }
    }

    public interface IRateLimitTable<in TId>
    {
        Task<bool> Attempt(TId id);
    }
}
