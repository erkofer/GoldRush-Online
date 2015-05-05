using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisEntityHashTable<TEntity, TId, TField>
        : RedisEntityTableBase<TEntity, TId>, IEntityHashTable<TEntity, TId, TField>
    {
        readonly IDatabase _db;
        readonly ISerializer<TField> _fieldSerializer;
        private readonly IIdentifier<TEntity, TField> _fieldIdentifier;

        public RedisEntityHashTable(IDatabaseArea db, ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier, ISerializer<TField> fieldSerializer, IIdentifier<TEntity, TField> fieldIdentifier)
            : base(serializer, keySerializer, identifier)
        {
            _db = db;
            _fieldSerializer = fieldSerializer;
            _fieldIdentifier = fieldIdentifier;
        }

        public Task<bool> Delete(TId id, TField member)
        {
            var tid = KeySerializer.Serialize(id);
            var mem = _fieldSerializer.Serialize(member);
            return _db.HashDeleteAsync(tid, mem);
        }

        public Task<long> Delete(TId id, TField[] members)
        {
            var tid = KeySerializer.Serialize(id);
            var mems = new RedisValue[members.Length];
            for (var i = 0; i < members.Length; i++)
                mems[i] = _fieldSerializer.Serialize(members[i]);
            return _db.HashDeleteAsync(tid, mems);
        }

        public Task<bool> Exists(TId id, TField member)
        {
            var tid = KeySerializer.Serialize(id);
            var mem = _fieldSerializer.Serialize(member);
            return _db.HashExistsAsync(tid, mem);
        }

        public async Task<TEntity> Get(TId id, TField field)
        {
            var tid = KeySerializer.Serialize(id);
            var mem = _fieldSerializer.Serialize(field);
            var result = await _db.HashGetAsync(tid, mem);
            return Deserialize(result, id, field);
        }

        public async Task<TEntity[]> Get(TId id, TField[] members)
        {
            var tid = KeySerializer.Serialize(id);
            var mems = new RedisValue[members.Length];
            for (var i = 0; i < members.Length; i++)
                mems[i] = _fieldSerializer.Serialize(members[i]);
            var result = await _db.HashGetAsync(tid, mems);
            var ret = new TEntity[result.Length];
            for (var i = 0; i < result.Length; i++)
                ret[i] = Deserialize(result[i], id, members[i]);
            return ret;
        }

        public async Task<TEntity[]> GetAll(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            var result = await _db.HashGetAllAsync(tid);
            var ret = new TEntity[result.Length];
            for (var i = 0; i < result.Length; i++)
            {
                var hash = result[i];
                ret[i] = Deserialize(hash.Value, id, 
                    _fieldSerializer.Deserialize(hash.Name));
            }
            return ret;
        }

        public async Task<TField[]> Fields(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            var result = await _db.HashKeysAsync(tid);
            var ret = new TField[result.Length];
            for (var i = 0; i < result.Length; i++)
                ret[i] = _fieldSerializer.Deserialize(result[i]);
            return ret;
        }

        public Task<long> Length(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            return _db.HashLengthAsync(tid);
        }

        public Task<bool> Set(TEntity entity)
        {// the only method that matters
            var serial = Serializer.Serialize(entity);
            var tid = KeySerializer.Serialize(Identifier.GetId(entity));
            var field = _fieldSerializer.Serialize(_fieldIdentifier.GetId(entity));
            return _db.HashSetAsync(tid, field, serial);
        }

        public Task Set(TId id, TEntity[] entities)
        {
            var tid = KeySerializer.Serialize(id);
            var hashes = new HashEntry[entities.Length];
            for (var i = 0; i < hashes.Length; i++)
            {
                var ent = entities[i];
                var field = _fieldSerializer.Serialize(_fieldIdentifier.GetId(ent));
                var serial = Serializer.Serialize(ent);
                hashes[i] = new HashEntry(field, serial);
            }
            return _db.HashSetAsync(tid, hashes);
        }

        public async Task<TEntity[]> Values(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            var result = await _db.HashValuesAsync(tid);
            var ret = new TEntity[result.Length];
            for (var i = 0; i < result.Length; i++)
                ret[i] = Deserialize(result[i], id);
            return ret;
        }

        private TEntity Deserialize(RedisValue result, TId id, TField field)
        {
            if (result.IsNull)
                return default(TEntity);
            var ent = Deserialize(result, id);
            _fieldIdentifier.SetId(ent, field);
            return ent;
        }
    }
}
