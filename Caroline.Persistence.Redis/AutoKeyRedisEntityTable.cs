using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class AutoKeyRedisEntityTable<TEntity> : IAutoKeyEntityTable<TEntity>
    {
        readonly IDatabase _db;
        readonly ILongTable _idIncrementDb;
        readonly long _idIncrementKey;
        readonly ISerializer<TEntity> _serializer;
        readonly IIdentifier<TEntity, long> _identifier;
        readonly CarolineScriptsRepo _scripts;
        readonly TimeSpan? _defaultExpiry;

        public AutoKeyRedisEntityTable(IDatabaseArea db, ILongTable idIncrementDb, long idIncrementKey, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier, TimeSpan? defaultExpiry = null)
        {
            _db = db.Area;
            _scripts = db.Scripts;
            _idIncrementDb = idIncrementDb;
            _idIncrementKey = idIncrementKey;
            _serializer = serializer;
            _identifier = identifier;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(long id)
        {
            var entity = await _db.StringGetAsync(VarintBitConverter.GetVarintBytes(id));
            return entity.HasValue ? _serializer.Deserialize(entity) : default(TEntity);
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
            var result = await _db.StringSetAsync(key, value, expiry ?? _defaultExpiry, when);
            if (mode == SetMode.Add && result == false)
                await LogUtility.LogMessage(
                    "AutoIncrementRedisEntityTable.Set(SetMode.Add) failed. " +
                    "Retrieved a unique autoincrement id, then it was taken");
            return result;
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var key = VarintBitConverter.GetVarintBytes(_identifier.GetId(entity));
            var value = _serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previous;
            if (expiry == null)
                previous = await _db.StringGetSetAsync(key, value);
            else
                previous = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);
            return previous.HasValue ? _serializer.Deserialize(previous) : default(TEntity);
        }

        public Task<bool> Delete(long id)
        {
            return _db.KeyDeleteAsync(VarintBitConverter.GetVarintBytes(id));
        }
    }
}
