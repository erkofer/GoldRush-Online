using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisEntityListTable<TEntity, TId> : RedisEntityTableBase<TEntity, TId>, IEntityListTable<TEntity, TId>
    {
        public RedisEntityListTable(IDatabaseArea db, ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier)
            : base(serializer, keySerializer, identifier)
        {
            Database = db;
        }

        public async Task<TEntity> GetByIndex(TId id, long index)
        {
            var key = KeySerializer.Serialize(id);
            var result = await Database.ListGetByIndexAsync(key, index);
            return Deserialize(result, id);
        }

        public Task<long> InsertAt(TId id, RedisValue pivot, RedisValue value, IndexSide side)
        {
            var key = KeySerializer.Serialize(id);
            switch (side)
            {
                case IndexSide.Left:
                    return Database.ListInsertBeforeAsync(key, pivot, value);
                case IndexSide.Right:
                    return Database.ListInsertAfterAsync(key, pivot, value);
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }

        public async Task<TEntity> Pop(TId id, IndexSide side)
        {
            var key = KeySerializer.Serialize(id);
            RedisValue result;
            switch (side)
            {
                case IndexSide.Left:
                    result = await Database.ListLeftPopAsync(key);
                    break;
                case IndexSide.Right:
                    result = await Database.ListRightPopAsync(key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
            return Deserialize(result, id);
        }

        public async Task<TEntity[]> Pop(TId id, IndexSide side, long count)
        {
            if(count < 0)
                throw new ArgumentException("count must be greater than or equal to 0.", "count");

            var key = KeySerializer.Serialize(id);
            var result = await Database.ListPopManyAsync(Database.Scripts, key, count, side);
            
            var ret = new TEntity[result.Length];
            for (var i = 0; i < ret.Length; i++)
                ret[i] = Deserialize(result[i], id);
            return ret;
        }

        public Task<long> Push(TEntity entity, IndexSide side)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var serial = Serializer.Serialize(entity);
            switch (side)
            {
                case IndexSide.Left:
                    return Database.ListLeftPushAsync(key, serial);
                case IndexSide.Right:
                    return Database.ListRightPushAsync(key, serial);
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }

        public Task<long> Length(TId id)
        {
            var tid = KeySerializer.Serialize(id);
            return Database.ListLengthAsync(tid);
        }

        public async Task<TEntity[]> Range(TId id, long start = 0, long stop = -1)
        {
            var key = KeySerializer.Serialize(id);
            var results = await Database.ListRangeAsync(key, start, stop);
            var ret = new TEntity[results.Length];
            for (var i = 0; i < results.Length; i++)
            {
                ret[i] = Deserialize(results[i], id);
            }
            return ret;
        }

        public Task<long> Remove(TId id, TEntity value, long count = 0)
        {
            var key = KeySerializer.Serialize(id);
            var serial = Serializer.Serialize(value);
            return Database.ListRemoveAsync(key, serial, count);
        }

        public async Task<TEntity> RightPopLeftPush(TId source, TId destination)
        {
            var tidSource = KeySerializer.Serialize(source);
            var tidDestination = KeySerializer.Serialize(destination);
            var result = await Database.ListRightPopLeftPushAsync(tidSource, tidDestination);
            return Deserialize(result, destination);
        }

        public Task SetByIndex(TEntity entity, long index)
        {
            var id = Identifier.GetId(entity);
            var key = KeySerializer.Serialize(id);
            var serial = Serializer.Serialize(entity);
            return Database.ListSetByIndexAsync(key, index, serial);
        }

        public Task Trim(TId id, long start, long stop)
        {
            var key = KeySerializer.Serialize(id);
            return Database.ListTrimAsync(key, start, stop);
        }

        public IDatabaseArea Database { get; private set; }
    }

    public enum IndexSide : byte
    {
        Left,
        Right
    }

    public interface IEntityListTable<TEntity, in TId> : IDatabaseTable
    {
        Task<TEntity> GetByIndex(TId id, long index);
        Task<long> InsertAt(TId id, RedisValue pivot, RedisValue value, IndexSide side);
        Task<TEntity> Pop(TId id, IndexSide side);
        Task<TEntity[]> Pop(TId id, IndexSide side, long count);
        Task<long> Push(TEntity entity, IndexSide side);
        Task<long> Length(TId id);
        Task<TEntity[]> Range(TId id, long start = 0, long stop = -1);
        Task<long> Remove(TId id, TEntity value, long count = 0);
        Task<TEntity> RightPopLeftPush(TId source, TId destination);
        Task SetByIndex(TEntity entity, long index);
        Task Trim(TId id, long start, long stop);
    }
}
