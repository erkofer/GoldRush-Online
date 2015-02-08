using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    /// <summary>
    /// A specialized internal table that tracks information of other tables in the database.
    /// </summary>
    class RedisPropertiesTable : RedisTable
    {
        private readonly IDatabase _db;

        public RedisPropertiesTable(IDatabase db, long typeId, byte[] additionalKeyPrefix = null)
            : base(db, typeId, additionalKeyPrefix)
        {
            _db = db;
        }

        byte[] GetRowKey(PropertyKeys key)
        {
            return GetRowKey(VarintBitConverter.GetVarintBytes((ulong)PropertyKeys.HighestFreeTypeId));
        }

        public async Task<ulong> GetFreeTypeId()
        {
            var key = GetRowKey(PropertyKeys.HighestFreeTypeId);
            var val = await _db.StringIncrementAsync(key); // always returns > 0
            return (ulong)val;
        }

        /// <summary>
        /// Returns the ulong id of the Types table, which may not exist yet in that keyspace.
        /// </summary>
        public async Task<long> GetTypesTable()
        {
            var key = GetRowKey(PropertyKeys.TypesTableTypeId);
            while (true)
            {
                var attempt = await _db.StringGetAsync(key);
                if (!attempt.IsNull)
                {
                    return VarintBitConverter.ToInt64(attempt);
                }
                //_db.ScriptEvaluateAsync()
            }
        }

        enum PropertyKeys : ulong
        {
            HighestFreeTypeId = 0,
            TypesTableTypeId = 1,
            TablesMaxIdTableTypeId = 2,
        }
    }
}
