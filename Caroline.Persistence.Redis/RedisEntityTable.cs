using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisEntityTable<TEntity, TId> : RedisEntityTableBase<TEntity, TId>, IEntityTable<TEntity, TId>
    {
        readonly TimeSpan? _defaultExpiry;

        public RedisEntityTable(IDatabaseArea db, ISerializer<TEntity> serializer, ISerializer<TId> keySerialzer, IIdentifier<TEntity, TId> identifier, TimeSpan? defaultExpiry = null)
            : base(serializer, keySerialzer, identifier)
        {
            Database = db;
            _defaultExpiry = defaultExpiry;
        }

        public async Task<TEntity> Get(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            var serial = await Database.StringGetAsync(tid);
            var entity = Deserialize(serial, id);
            return entity;
        }

        public async Task<bool> Set(TEntity entity, TimeSpan? expiry = null, When when = When.Always)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var value = Serializer.Serialize(entity);
            return await Database.StringSetAsync(key, value, expiry ?? _defaultExpiry, when);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var value = Serializer.Serialize(entity);
            expiry = expiry ?? _defaultExpiry;
            RedisValue previousSerial;
            if (expiry == null)
                previousSerial = await Database.StringGetSetAsync(key, value);
            else
                previousSerial = await Database.StringGetSetExpiryAsync(Database.Scripts, key, value, expiry.Value);

            return Deserialize(previousSerial, id);
        }

        public Task<bool> Delete(TId id)
        {
            return Database.KeyDeleteAsync(KeySerializer.Serialize(id));
        }

        public IDatabaseArea Database { get; private set; }
    }

    public abstract class RedisEntityTableBase<TEntity, TId> : RedisEntityTableBase<TId>
    {
        protected ISerializer<TEntity> Serializer { get; private set; }
        protected IIdentifier<TEntity, TId> Identifier { get; private set; }

        protected RedisEntityTableBase(ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier)
            : base(keySerializer)
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

    public abstract class RedisEntityTableBase<TId>
    {
        protected RedisEntityTableBase(ISerializer<TId> keySerializer)
        {
            KeySerializer = keySerializer;
        }

        protected ISerializer<TId> KeySerializer { get; private set; }
        
    }
}
