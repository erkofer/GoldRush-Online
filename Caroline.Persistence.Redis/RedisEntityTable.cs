using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisEntityTable<TEntity, TId> : RedisEntityTableBase<TEntity, TId>, IEntityTable<TEntity, TId>
    {
        readonly IDatabase _db;
        readonly CarolineScriptsRepo _scripts;
        readonly TimeSpan? _defaultExpiry;

        public RedisEntityTable(IDatabaseArea db, ISerializer<TEntity> serializer, ISerializer<TId> keySerialzer, IIdentifier<TEntity, TId> identifier, TimeSpan? defaultExpiry = null)
            : base(serializer, keySerialzer, identifier)
        {
            _db = db;
            _scripts = db.Scripts;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            var serial = await _db.StringGetAsync(tid);
            var entity = Deserialize(serial, id);
            return entity;
        }

        public async Task<bool> Set(TEntity entity, TimeSpan? expiry = null)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var value = Serializer.Serialize(entity);
            return await _db.StringSetAsync(key, value, expiry ?? _defaultExpiry);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var value = Serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previousSerial;
            if (expiry == null)
                previousSerial = await _db.StringGetSetAsync(key, value);
            else
                previousSerial = await _db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);

            return Deserialize(previousSerial, id);
        }

        public Task<bool> Delete(TId id)
        {
            return _db.KeyDeleteAsync(KeySerializer.Serialize(id));
        }
    }

    public abstract class RedisEntityTableBase<TEntity, TId>
    {
        public ISerializer<TEntity> Serializer { get; private set; }
        public ISerializer<TId> KeySerializer { get; private set; }
        public IIdentifier<TEntity, TId> Identifier { get; private set; }

        public RedisEntityTableBase(ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier)
        {
            Serializer = serializer;
            KeySerializer = keySerializer;
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
