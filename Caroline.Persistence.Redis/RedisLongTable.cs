using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisLongTable : ILongTable
    {
        readonly TimeSpan? _defaultExpiry;

        public RedisLongTable(IDatabaseArea db, TimeSpan? defaultExpiry)
        {
            _defaultExpiry = defaultExpiry;
            Database = db;
        }

        public void IncrementFaf(RedisKey id, long incrementValue = 1, TimeSpan? expiry = null)
        {
            var key = id;
            expiry = expiry ?? _defaultExpiry;
            if (expiry != null)
                Database.IncrementExpiryFaf(Database.Scripts, key, incrementValue, expiry.Value);
            else
                Database.StringIncrement(key, incrementValue, CommandFlags.FireAndForget);
        }

        public Task<long> IncrementAsync(RedisKey id, long incrementValue = 1, TimeSpan? expiry = null)
        {
            expiry = expiry ?? _defaultExpiry;
            return expiry != null
                ? Database.IncrementExpiryAsync(Database.Scripts, id, incrementValue, expiry.Value)
                : Database.StringIncrementAsync(id, incrementValue);
        }

        public async Task<long> Get(RedisKey id)
        {
            return (long)await Database.StringGetAsync(id);
        }

        public IDatabaseArea Database { get; private set; }
    }
}
