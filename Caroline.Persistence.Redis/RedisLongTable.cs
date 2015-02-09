using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisLongTable : ILongTable
    {
        readonly TimeSpan? _defaultExpiry;
        readonly IDatabase _db;
        readonly CarolineScriptsRepo _scripts;

        public RedisLongTable(IDatabaseArea db, TimeSpan? defaultExpiry)
        {
            _defaultExpiry = defaultExpiry;
            _db = db.Area;
            _scripts = db.Scripts;
        }

        public void IncrementFaf(long id, long incrementValue = 1, TimeSpan? expiry = null)
        {
            var key = VarintBitConverter.GetVarintBytes(id);
            expiry = expiry ?? _defaultExpiry;
            if (expiry != null)
                _db.IncrementExpiryFaf(_scripts, key, incrementValue, expiry.Value);
            else
                _db.StringIncrement(key, incrementValue, CommandFlags.FireAndForget);
        }

        public Task<long> IncrementAsync(long id, long incrementValue = 1, TimeSpan? expiry = null)
        {
            // key is entity type id, followed by entity id
            var key = VarintBitConverter.GetVarintBytes(id);
            expiry = expiry ?? _defaultExpiry;
            return expiry != null 
                ? _db.IncrementExpiryAsync(_scripts, key, incrementValue, expiry.Value) 
                : _db.StringIncrementAsync(key, incrementValue);
        }
    }
}
