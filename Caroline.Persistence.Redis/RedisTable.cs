using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisStringTable : IStringTable
    {
        readonly IDatabase _db;
        private readonly CarolineScriptsRepo _scripts;

        public RedisStringTable(IDatabaseArea db)
        {
            _db = db.Area;
            _scripts = db.Scripts;
        }

        public async Task<string> Get(string id)
        {
            return await _db.StringGetAsync(id);
        }

        public Task<bool> Set(string id, string value, TimeSpan? expiry = null)
        {
            return _db.StringSetAsync(id, value, expiry);
        }

        public async Task<string> GetSet(string id, string setValue, TimeSpan? expiry = null)
        {
            if (expiry.HasValue)
                return await _db.StringGetSetExpiryAsync(_scripts, id, setValue, expiry.Value);
            return await _db.StringGetSetAsync(id, setValue);
        }

        public Task<bool> Delete(string id)
        {
            return _db.KeyDeleteAsync(id);
        }
    }
}
