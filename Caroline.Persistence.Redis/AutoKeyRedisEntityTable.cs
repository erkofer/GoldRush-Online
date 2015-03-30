using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class AutoKeyRedisEntityTable<TEntity> : RedisEntityTableBase<TEntity, long>, IAutoKeyEntityTable<TEntity>
    {
        readonly IDatabase _db;
        readonly ILongTable _idIncrementDb;
        readonly RedisKey _idIncrementKey;
        readonly CarolineScriptsRepo _scripts;
        readonly TimeSpan? _defaultExpiry;

        public AutoKeyRedisEntityTable(IDatabaseArea db, ILongTable idIncrementDb, RedisKey idIncrementKey, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier, TimeSpan? defaultExpiry = null)
            : base(serializer, new LongIdentifier<TEntity>(identifier))
        {
            _db = db;
            _scripts = db.Scripts;
            _idIncrementDb = idIncrementDb;
            _idIncrementKey = idIncrementKey;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(TEntity id)
        {
            var longId = Identifier.GetId(id);
            var entity = await _db.StringGetAsync(longId);
            return Deserialize(entity, longId);
        }

        public async Task<bool> Set(TEntity entity, SetMode mode, TimeSpan? expiry = null)
        {
            string key;
            When when;
            switch (mode)
            {
                case SetMode.Add:
                    when = When.NotExists;
                    var longKey = await _idIncrementDb.IncrementAsync(_idIncrementKey);
                    key = longKey.ToStringInvariant();
                    Identifier.SetId(entity, key);
                    break;
                case SetMode.Overwrite:
                    when = When.Exists;
                    key = Identifier.GetId(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            var value = Serializer.Serialize(entity);
            var result = await _db.StringSetAsync(key, value, expiry ?? _defaultExpiry, when);
            if (mode == SetMode.Add && result == false)
                Debug.Fail(
                    "AutoIncrementRedisEntityTable.Set(SetMode.Add) failed. " +
                    "Retrieved a unique autoincrement id, then it was taken");
            return result;
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var key = Identifier.GetId(entity);
            var value = Serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previous;
            if (expiry == null)
                previous = await _db.StringGetSetAsync(key, value);
            else
                previous = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);
            return Deserialize(previous, key);
        }

        public Task<bool> Delete(TEntity id)
        {
            return _db.KeyDeleteAsync(Identifier.GetId(id));
        }
    }
}
