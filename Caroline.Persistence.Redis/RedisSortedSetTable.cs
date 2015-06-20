using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class RedisSortedSetTable<TEntity, TId> : RedisEntityTableBase<TEntity, TId>, ISortedSetTable<TEntity, TId>
    {
        readonly IDatabaseArea _db;
        readonly IIdentifier<TEntity, double> _scoreIdentifier;
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RedisSortedSetTable(IDatabaseArea db, ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier, IIdentifier<TEntity, double> scoreIdentifier)
            : base(serializer, keySerializer, identifier)
        {
            _db = db;
            _scoreIdentifier = scoreIdentifier;
        }

        public Task<bool> Add(TEntity entity)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var score = _scoreIdentifier.GetId(entity);
            var serial = Serializer.Serialize(entity);
            return _db.SortedSetAddAsync(id, serial, score);
        }

        public Task<bool> Push(TEntity entity, IndexSide side, double scoreOffset = 1)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var serial = Serializer.Serialize(entity);
            return _db.SortedSetPush(_db.Scripts, id, serial, side, scoreOffset);
        }

        public Task<long> CombineAndStore(SetOperation operation, TId destination, TId[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum)
        {
            var destId = KeySerializer.Serialize(destination);
            var keysSerial = new RedisKey[keys.Length];
            for (var i = 0; i < keys.Length; i++)
                keysSerial[i] = KeySerializer.Serialize(keys[i]);
            return _db.SortedSetCombineAndStoreAsync(operation, destId, keysSerial, weights, aggregate);
        }

        public Task<double> Increment(TEntity entity, double value = 1)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var serial = Serializer.Serialize(entity);
            return value > 0
                ? _db.SortedSetIncrementAsync(id, serial, value)
                : _db.SortedSetDecrementAsync(id, serial, value);
        }

        public Task<long> Length(TId key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None)
        {
            var id = KeySerializer.Serialize(key);
            return _db.SortedSetLengthAsync(id, min, max, exclude);
        }

        //lengthByValue doesnt make sense when sorting by the serialized format of an entity
        //public Task<long> LengthByValue(TId key, TEntity min, TEntity max, Exclude exclude = Exclude.None)
        //{
        //    var id = KeySerializer.Serialize(key);
        //    var minSerial = Serializer.Serialize(min);
        //    var maxSerial = Serializer.Serialize(max);
        //    return _db.SortedSetLengthByValueAsync(id, minSerial, maxSerial, exclude);
        //}

        public async Task<TEntity> Get(TId id, long rank)
        {
            var result = await RangeByScore(id, rank, rank);
            if (result.Length > 1)
                Log.Warn("Redis SortedSetRangeByRankWithScoresAsync returned >1 result when a range of 1 was given.");
            return result.Length > 0 ? result[1] : default(TEntity);
        }
        public async Task<TEntity[]> RangeByRank(TId id, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            var tid = KeySerializer.Serialize(id);
            var result = await _db.SortedSetRangeByRankWithScoresAsync(tid, start, stop, order);
            var ret = new TEntity[result.Length];
            for (var i = 0; i < ret.Length; i++)
                ret[i] = Deserialize(result[i], id);
            return ret;
        }

        public async Task<TEntity[]> RangeByScore(TId id,  double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            var tid = KeySerializer.Serialize(id);
            var result = await _db.SortedSetRangeByScoreWithScoresAsync(tid, start, stop, exclude, order, skip, take, flags);
            var ret = new TEntity[result.Length];
            for (var i = 0; i < ret.Length; i++)
                ret[i] = Deserialize(result[i], id);
            return ret;
        }

        public Task<long?> Rank(TEntity entity, Order order = Order.Ascending)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var serial = Serializer.Serialize(entity);
            return _db.SortedSetRankAsync(id, serial, order);
        }

        public Task<bool> Remove(TEntity entity)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var serial = Serializer.Serialize(entity);
            return _db.SortedSetRemoveAsync(id, serial);
        }

        public Task<long> RemoveRangeByRank(TId id, long start, long stop)
        {
            var tid = KeySerializer.Serialize(id);
            return _db.SortedSetRemoveRangeByRankAsync(tid, start, stop);
        }

        public Task<long> RemoveRangeByScore(TId id, double start, double stop)
        {
            var tid = KeySerializer.Serialize(id);
            return _db.SortedSetRemoveRangeByScoreAsync(tid, start, stop);
        }

        public Task<double?> Score(TEntity entity)
        {
            var id = KeySerializer.Serialize(Identifier.GetId(entity));
            var serial = Serializer.Serialize(entity);
            return _db.SortedSetScoreAsync(id, serial);
        }

        // missing a lot of commands

        public TEntity Deserialize(SortedSetEntry entry, TId id)
        {
            var ent = Deserialize(entry.Element, id);
            _scoreIdentifier.SetId(ent, entry.Score);
            return ent;
        }
    }

    public interface ISortedSetTable<TEntity, TId>
    {
        Task<bool> Add(TEntity entity);
        Task<bool> Push(TEntity entity, IndexSide side, double scoreOffset = 1);
        Task<long> CombineAndStore(SetOperation operation, TId destination, TId[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum);
        Task<double> Increment(TEntity entity, double value = 1);
        Task<long> Length(TId key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);
        Task<long?> Rank(TEntity entity, Order order = Order.Ascending);
        Task<bool> Remove(TEntity entity);
        Task<long> RemoveRangeByRank(TId id, long start, long stop);
        Task<long> RemoveRangeByScore(TId id, double start, double stop);
        Task<double?> Score(TEntity entity);
        Task<TEntity> Get(TId id, long rank);

        Task<TEntity[]> RangeByScore(TId id,  double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

        Task<TEntity[]> RangeByRank(TId id, long start = 0, long stop = -1, Order order = Order.Ascending);
    }
}
