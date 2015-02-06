using System;
using Caroline.Persistence.Extensions;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Caroline.Persistence
{
    /// <summary>
    /// Wraps a Redis database. Uses ProtoBuf serialization as an object-relational mapper.
    /// </summary>
    public abstract class RedisTable : IDatabaseTable
    {
        readonly byte[] _keyPrefix;

        public RedisTable(IDatabase db, long typeId, byte[] additionalKeyPrefix = null)
        {
            if (db == null)
                throw new ArgumentNullException("db");

            var pre = VarintBitConverter.GetVarintBytes(typeId);
            _keyPrefix = additionalKeyPrefix != null
                ? ArrayEx.Combine(additionalKeyPrefix, pre)
                : pre;
            Db = db;
        }

        protected byte[] GetRowKey([NotNull] byte[] keySuffix)
        {
            if (keySuffix == null) throw new ArgumentNullException("keySuffix");
            return ArrayEx.Combine(_keyPrefix, keySuffix);
        }

        protected IDatabase Db { get; private set; }
    }
}
