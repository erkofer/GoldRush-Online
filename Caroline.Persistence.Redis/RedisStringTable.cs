using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisStringTable : IStringTable
    {
        private readonly TimeSpan? _defaultExpiry;

        public RedisStringTable(IDatabaseArea db, TimeSpan? defaultExpiry = null)
        {
            Database = db;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<string> Get(string id)
        {
            return await Database.StringGetAsync(id);
        }

        public Task<bool> Set(string id, string value, TimeSpan? expiry = null, When when = When.Always)
        {
            return Database.StringSetAsync(id, value, expiry ?? _defaultExpiry, when);
        }

        public async Task<string> GetSet(string id, string setValue, TimeSpan? expiry = null)
        {
            expiry = expiry ?? _defaultExpiry;
            if (expiry.HasValue)
                return await Database.StringGetSetExpiryAsync(Database.Scripts, id, setValue, expiry.Value);
            return await Database.StringGetSetAsync(id, setValue);
        }

        public Task<bool> Delete(string id)
        {
            return Database.KeyDeleteAsync(id);
        }

        public IDatabaseArea Database { get; private set; }
    }
}
