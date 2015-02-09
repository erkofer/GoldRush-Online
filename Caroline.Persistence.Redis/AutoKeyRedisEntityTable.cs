using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class AutoKeyRedisEntityTable<TEntity> : IEntityTable<TEntity>
    {
        readonly IDatabase _db;
        readonly ILongTable _idIncrementDb;
        readonly long _idIncrementKey;
        readonly ISerializer<TEntity> _serializer;
        readonly IIdentifier<TEntity, long> _identifier;
        readonly CarolineScriptsRepo _scripts;

        public AutoKeyRedisEntityTable(IDatabaseArea db, ILongTable idIncrementDb, long idIncrementKey, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier)
        {
            _db = db.Area;
            _scripts = db.Scripts;
            _idIncrementDb = idIncrementDb;
            _idIncrementKey = idIncrementKey;
            _serializer = serializer;
            _identifier = identifier;
        }

        public async Task<bool> Set(TEntity entity, SetMode mode, TimeSpan? expiry = null)
        {
            byte[] key;
            When when;
            switch (mode)
            {
                case SetMode.Add:
                    when = When.NotExists;
                    var longKey = await _idIncrementDb.IncrementAsync(_idIncrementKey);
                    _identifier.SetId(entity, longKey);
                    key = VarintBitConverter.GetVarintBytes(longKey);
                    break;
                case SetMode.Overwrite:
                    when = When.Exists;
                    key = VarintBitConverter.GetVarintBytes(_identifier.GetId(entity));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            var value = _serializer.Serialize(entity);
            return await _db.StringSetAsync(key, value, expiry, when);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var key = VarintBitConverter.GetVarintBytes(_identifier.GetId(entity));
            var value = _serializer.Serialize(entity);
            RedisValue previous;
            if (expiry == null)
                previous = await _db.StringGetSetAsync(key, value);
            else
                previous = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);
            return previous.HasValue ? _serializer.Deserialize(previous) : default(TEntity);
        }
    }
}
