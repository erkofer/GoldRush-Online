using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisEntityTable<TEntity> : RedisTable, IEntityTable<TEntity>
    {
        readonly ISerializer<TEntity> _serializer;
        readonly IIdentifier<TEntity> _identifier;
        readonly Func<Task<long>> _newEntityIdFunc;
        readonly CarolineScriptsRepo _scripts;

        public RedisEntityTable(IDatabase db, ISerializer<TEntity> serializer, IIdentifier<TEntity> identifier, long typeId, Func<Task<long>> newEntityIdFunc, CarolineScriptsRepo scripts, byte[] additionalKeyprefix = null)
            : base(db, typeId, additionalKeyprefix)
        {
            _serializer = serializer;
            _identifier = identifier;
            _newEntityIdFunc = newEntityIdFunc;
            _scripts = scripts;
        }

        //public abstract Task<IEnumerable<TEntity>> Get();
        //public abstract Task<TEntity> Get(ulong id);

        //public abstract Task<TEntity> Add(TEntity entity);
        //public abstract void Remove(TEntity entity);
        //public abstract void Remove(ulong id);

        private byte[] GetRowKey(TEntity entity)
        {
            // key is entity type id, followed by entity id
            return GetRowKey(VarintBitConverter.GetVarintBytes(_identifier.GetId(entity)));
        }

        public async Task<bool> Set(TEntity entity, SetMode mode, TimeSpan? expiry = null)
        {
            if (mode == SetMode.Overwrite && _identifier.GetId(entity) <= 0)
                throw new ArgumentOutOfRangeException();
            if (mode == SetMode.Add)
                _identifier.SetId(entity, await _newEntityIdFunc());
            var key = GetRowKey(entity);
            var value = _serializer.Serialize(entity);
            When when;
            switch (mode)
            {
                case SetMode.Add:
                    when = When.NotExists;
                    break;
                case SetMode.Overwrite:
                    when = When.Exists;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            return await Db.StringSetAsync(key, value, expiry, when);
        }

        public async Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null)
        {
            if (_identifier.GetId(entity) <= 0)
                throw new ArgumentOutOfRangeException();
            var key = GetRowKey(entity);
            var value = _serializer.Serialize(entity);
            RedisValue previous;
            if (expiry == null)
                previous = await Db.StringGetSetAsync(key, value);
            else
                previous = await Db.StringGetSetExpiryAsync(_scripts, key, value, expiry.Value);
            return previous.HasValue ? _serializer.Deserialize(previous) : default(TEntity);
        }
    }
}
