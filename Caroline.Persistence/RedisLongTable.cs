using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence
{
    public class RedisLongTable : RedisTable
    {
        public RedisLongTable(IDatabase db, long typeId, byte[] additionalKeyPrefix = null)
            : base(db, typeId, additionalKeyPrefix)
        {
        }

        byte[] GetRowKey(long id)
        {
            return GetRowKey(VarintBitConverter.GetVarintBytes(id));
        }

        public void IncrementFaf(long id, long incrementValue)
        {
            var key = GetRowKey(id);
            Db.StringIncrement(key, incrementValue, CommandFlags.FireAndForget);
        }

        public Task<long> IncrementAsync(long id, long incrementValue = 1)
        {
            // key is entity type id, followed by entity id
            var key = GetRowKey(id);
            return Db.StringIncrementAsync(key, incrementValue);
        }
    }
}
