using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisEntityTable<TEntity> : IEntityTable<TEntity>
    {
        readonly IDatabase _db;
        readonly ISerializer<TEntity> _serializer;
        readonly IIdentifier<TEntity, byte[]> _identifier;
        readonly CarolineScriptsRepo _scripts;
        readonly TimeSpan? _defaultExpiry;

        public RedisEntityTable(IDatabaseArea db, ISerializer<TEntity> serializer, IIdentifier<TEntity, byte[]> identifier, TimeSpan? defaultExpiry = null)
        {
            _db = db.Area;
            _scripts = db.Scripts;
            _serializer = serializer;
            _identifier = identifier;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(TEntity id)
        {
            var entity = await _db.StringGetAsync(_identifier.GetId(id));
            return _serializer.Deserialize(entity);
        }

        public async Task<bool> Set(TEntity entity, TimeSpan? expiry = null)
        {
            var key = _identifier.GetId(entity);
            var value = _serializer.Serialize(entity);
            return await _db.StringSetAsync(key, value, expiry ?? _defaultExpiry);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var key = _identifier.GetId(entity);
            var value = _serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previous;
            if (expiry == null)
                previous = await _db.StringGetSetAsync(key, value);
            else
                previous = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);
            return previous.HasValue ? _serializer.Deserialize(previous) : default(TEntity);
        }

        public Task<bool> Delete(TEntity entity)
        {
            return _db.KeyDeleteAsync(_identifier.GetId(entity));
        }
    }
}
