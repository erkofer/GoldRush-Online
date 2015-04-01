using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisEntityListTable<TEntity> : RedisEntityTableBase<TEntity, string>, IEntityListTable<TEntity>
    {
        private readonly IDatabase _db;

        public RedisEntityListTable(IDatabase db, ISerializer<TEntity> serializer, IIdentifier<TEntity, RedisKey> identifier)
            : base(serializer, identifier)
        {
            _db = db;
        }

        public async Task<TEntity> GetByIndex(TEntity key, long index)
        {
            var tid = Identifier.GetId(key);
            var result = await _db.ListGetByIndexAsync(tid, index);
            return Deserialize(result, tid);
        }

        public Task<long> InsertAt(TEntity key, RedisValue pivot, RedisValue value,IndexSide side)
        {
            var tid = Identifier.GetId(key);
            switch (side)
            {
                case IndexSide.Left:
                    return _db.ListInsertBeforeAsync(tid, pivot, value);
                case IndexSide.Right:
                    return _db.ListInsertAfterAsync(tid, pivot, value);
                    default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }

        public async Task<TEntity> Pop(TEntity key, RedisValue value, IndexSide side)
        {
            var tid = Identifier.GetId(key);
            RedisValue result;
            switch (side)
            {
                case IndexSide.Left:
                    result = await _db.ListLeftPopAsync(tid);
                    break;
                case IndexSide.Right:
                    result = await _db.ListRightPopAsync(tid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
            return Deserialize(result, tid);
        }

        public Task<long> Push(TEntity key, RedisValue value, IndexSide side)
        {
            var tid = Identifier.GetId(key);
            switch (side)
            {
                case IndexSide.Left:
                    return _db.ListLeftPushAsync( tid, value);
                case IndexSide.Right:
                    return _db.ListRightPushAsync(tid, value);
                default:
                    throw new ArgumentOutOfRangeException("side");
            }
        }

        public Task<long> Length(TEntity key)
        {
            var tid = Identifier.GetId(key);
            return _db.ListLengthAsync(tid);
        }

        public async Task<TEntity[]> Range(TEntity key, long start = 0, long stop = -1)
        {
            var tid = Identifier.GetId(key);
            var results = await _db.ListRangeAsync(tid, start, stop);
            var ret = new TEntity[results.Length];
            for (var i = 0; i < results.Length; i++)
            {
                ret[i] = Deserialize(results[i], tid);
            }
            return ret;
        }

        public Task<long> Remove(TEntity key, RedisValue value, long count = 0)
        {
            var tid = Identifier.GetId(key);
            return _db.ListRemoveAsync(tid, value, count);
        }

        public async Task<TEntity> RightPushLeftPop(TEntity source, TEntity destination)
        {
            var tidSource = Identifier.GetId(source);
            var tidDestination = Identifier.GetId(destination);
            var result = await _db.ListRightPopLeftPushAsync(tidSource, tidDestination);
            return Deserialize(result, tidDestination);
        }

        public Task SetByIndex(TEntity entity, long index)
        {
            var tid = Identifier.GetId(entity);
            var serial = Serializer.Serialize(entity);
            return _db.ListSetByIndexAsync(tid, index, serial);
        }

        public Task Trim(TEntity key, long start, long stop)
        {
            var tid = Identifier.GetId(key);
            return _db.ListTrimAsync(tid, start, stop);
        }
    }

    public enum IndexSide : byte
    {
        Left,
        Right
    }

    public interface IEntityListTable<TEntity>
    {
        Task<TEntity> GetByIndex(TEntity key, long index);
        Task<long> InsertAt(TEntity key, RedisValue pivot, RedisValue value,IndexSide side);
        Task<TEntity> Pop(TEntity key, RedisValue value, IndexSide side);
        Task<long> Push(TEntity key, RedisValue value, IndexSide side);
        Task<long> Length(TEntity key);
        Task<TEntity[]> Range(TEntity key, long start = 0, long stop = -1);
        Task<long> Remove(TEntity key, RedisValue value, long count = 0);
        Task<TEntity> RightPushLeftPop(TEntity source, TEntity destination);
        Task SetByIndex(TEntity entity, long index);
        Task Trim(TEntity key, long start, long stop);
    }
}
