using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisLongTable : ILongTable
    {
        readonly IDatabase _db;
        readonly CarolineScriptsRepo _scripts;

        public RedisLongTable(IDatabaseArea db)
        {
            _db = db.Area;
            _scripts = db.Scripts;
        }

        public void IncrementFaf(long id, long incrementValue)
        {
            var key = VarintBitConverter.GetVarintBytes(id);
            _db.StringIncrement(key, incrementValue, CommandFlags.FireAndForget);
        }

        public Task<long> IncrementAsync(long id, long incrementValue = 1)
        {
            // key is entity type id, followed by entity id
            var key = VarintBitConverter.GetVarintBytes(id);
            return _db.StringIncrementAsync(key, incrementValue);
        }

        public IDatabase GetKey(long id)
        {
            return new DatabaseWrapper(_db, VarintBitConverter.GetVarintBytes(id), _scripts);
        }
    }
}
