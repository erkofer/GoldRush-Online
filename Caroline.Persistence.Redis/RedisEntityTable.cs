using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisEntityTable<TEntity> : RedisEntityTableBase<TEntity, string>, IEntityTable<TEntity>
    {
        readonly IDatabase _db;
        readonly CarolineScriptsRepo _scripts;
        readonly TimeSpan? _defaultExpiry;

        public RedisEntityTable(IDatabaseArea db, ISerializer<TEntity> serializer, IIdentifier<TEntity, string> identifier, TimeSpan? defaultExpiry = null)
            : base(serializer, identifier)
        {
            _db = db;
            _scripts = db.Scripts;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(TEntity id)
        {
            var tid = Identifier.GetId(id);
            var serial = await _db.StringGetAsync(tid);
            var entity = Deserialize(serial, tid);
            return entity;
        }

        public async Task<bool> Set(TEntity entity, TimeSpan? expiry = null)
        {
            var key = Identifier.GetId(entity);
            var value = Serializer.Serialize(entity);
            return await _db.StringSetAsync(key, value, expiry ?? _defaultExpiry);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var key = Identifier.GetId(entity);
            var value = Serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previousSerial;
            if (expiry == null)
                previousSerial = await _db.StringGetSetAsync(key, value);
            else
                previousSerial = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);

            return Deserialize(previousSerial, key);
        }

        public Task<bool> Delete(TEntity entity)
        {
            return _db.KeyDeleteAsync(Identifier.GetId(entity));
        }
    }

    public abstract class RedisEntityTableBase<TEntity, TId>
    {
        public ISerializer<TEntity> Serializer { get; private set; }
        public IIdentifier<TEntity, TId> Identifier { get; private set; }

        public RedisEntityTableBase(ISerializer<TEntity> serializer, IIdentifier<TEntity, TId> identifier)
        {
            Serializer = serializer;
            Identifier = identifier;
        }

        protected TEntity Deserialize(RedisValue value, TId id)
        {
            if (value.IsNull) return default(TEntity);

            var previous = Serializer.Deserialize(value);
            Identifier.SetId(previous, id);
            return previous;
        }
    }
}
