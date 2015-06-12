using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class DatabaseWrapper : IDatabaseArea
    {
        [NotNull]
        readonly IDatabaseArea _impl;
        readonly RedisKey _prefix;
        readonly CarolineScriptsRepo _scripts;

        public DatabaseWrapper([NotNull] IDatabaseArea impl, RedisKey prefix)
        {
            if (impl == null) throw new ArgumentNullException("impl");
            _impl = impl;
#pragma warning disable 612 // suppressed. RedisKey.Prepend() is prefered, but that mutates the RedisKey. + is pure but creates object allocations
            _prefix = prefix + ":";
#pragma warning restore 612
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            return _impl.PingAsync(flags);
        }

        public bool TryWait(Task task)
        {
            return _impl.TryWait(task);
        }

        public void Wait(Task task)
        {
            _impl.Wait(task);
        }

        public T Wait<T>(Task<T> task)
        {
            return _impl.Wait(task);
        }

        public void WaitAll(params Task[] tasks)
        {
            _impl.WaitAll(tasks);
        }

        public ConnectionMultiplexer Multiplexer
        {
            get { return _impl.Multiplexer; }
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            return _impl.Ping(flags);
        }

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.DebugObjectAsync(key, flags);
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDecrementAsync(key.Prepend(_prefix), hashField, value, flags);
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDecrementAsync(key.Prepend(_prefix), hashField, value, flags);
        }

        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDeleteAsync(key.Prepend(_prefix), hashField, flags);
        }

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDeleteAsync(key.Prepend(_prefix), hashFields, flags);
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashExistsAsync(key.Prepend(_prefix), hashField, flags);
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGetAllAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGetAsync(key.Prepend(_prefix), hashField, flags);
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGetAsync(key.Prepend(_prefix), hashFields, flags);
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashIncrementAsync(key.Prepend(_prefix), hashField, value, flags);
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashIncrementAsync(key.Prepend(_prefix), hashField, value, flags);
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashKeysAsync(key.Prepend(_prefix), flags);
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashLengthAsync(key.Prepend(_prefix), flags);
        }

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashSetAsync(key.Prepend(_prefix), hashFields, flags);
        }

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashSetAsync(key.Prepend(_prefix), hashField, value, when, flags);
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashValuesAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogAddAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogAddAsync(key.Prepend(_prefix), values, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogLengthAsync(key.Prepend(_prefix), flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.HyperLogLogLengthAsync(keys, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            return _impl.IdentifyEndpointAsync(key.Prepend(_prefix), flags);
        }

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.IsConnected(key.Prepend(_prefix), flags);
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyDeleteAsync(key.Prepend(_prefix), flags);
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.KeyDeleteAsync(keys, flags);
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyDumpAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExistsAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExpireAsync(key.Prepend(_prefix), expiry, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExpireAsync(key.Prepend(_prefix), expiry, flags);
        }

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0,
            MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyMigrateAsync(key.Prepend(_prefix), toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyMoveAsync(key.Prepend(_prefix), database, flags);
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyPersistAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyRandomAsync(flags);
        }

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyRenameAsync(key.Prepend(_prefix), newKey, when, flags);
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyRestoreAsync(key.Prepend(_prefix), value, expiry, flags);
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyTimeToLiveAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyTypeAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListGetByIndexAsync(key.Prepend(_prefix), index, flags);
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListInsertAfterAsync(key.Prepend(_prefix), pivot, value, flags);
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListInsertBeforeAsync(key.Prepend(_prefix), pivot, value, flags);
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPopAsync(key.Prepend(_prefix), flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPushAsync(key.Prepend(_prefix), value, when, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPushAsync(key.Prepend(_prefix), values, flags);
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLengthAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRangeAsync(key.Prepend(_prefix), start, stop, flags);
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRemoveAsync(key.Prepend(_prefix), value, count, flags);
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPopAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPushAsync(key.Prepend(_prefix), value, when, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPushAsync(key.Prepend(_prefix), values, flags);
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListSetByIndexAsync(key.Prepend(_prefix), index, value, flags);
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListTrimAsync(key.Prepend(_prefix), start, stop, flags);
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockExtendAsync(key.Prepend(_prefix), value, expiry, flags);
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockQueryAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockReleaseAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockTakeAsync(key.Prepend(_prefix), value, expiry, flags);
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            return _impl.PublishAsync(channel, message, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (keys != null)
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = keys[i].Prepend(_prefix);
                }
            return _impl.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (keys != null)
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = keys[i].Prepend(_prefix);
                }
            return _impl.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetAddAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetAddAsync(key.Prepend(_prefix), values, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetCombineAndStoreAsync(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SetCombineAndStoreAsync(operation, destination.Prepend(_prefix), keys, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetCombineAsync(operation, first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SetCombineAsync(operation, keys, flags);
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetContainsAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetLengthAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetMembersAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetMoveAsync(source.Prepend(_prefix), destination.Prepend(_prefix), value, flags);
        }

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetPopAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRandomMemberAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRandomMembersAsync(key.Prepend(_prefix), count, flags);
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRemoveAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRemoveAsync(key.Prepend(_prefix), values, flags);
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending,
            SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortAndStoreAsync(destination.Prepend(_prefix), key.Prepend(_prefix), skip, take, order, sortType, @by, get, flags);
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric,
            RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortAsync(key.Prepend(_prefix), skip, take, order, sortType, @by, get, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetAddAsync(key.Prepend(_prefix), member, score, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetAddAsync(key.Prepend(_prefix), values, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetCombineAndStoreAsync(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys,
            double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SortedSetCombineAndStoreAsync(operation, destination.Prepend(_prefix), keys, weights, aggregate, flags);
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetDecrementAsync(key.Prepend(_prefix), member, value, flags);
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetIncrementAsync(key.Prepend(_prefix), member, value, flags);
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetLengthAsync(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetLengthByValueAsync(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByRankAsync(key.Prepend(_prefix), start, stop, order, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByRankWithScoresAsync(key.Prepend(_prefix), start, stop, order, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None,
            Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByScoreAsync(key.Prepend(_prefix), start, stop, exclude, order, skip, take, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByScoreWithScoresAsync(key.Prepend(_prefix), start, stop, exclude, order, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(),
            Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByValueAsync(key.Prepend(_prefix), min, max, exclude, skip, take, flags);
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRankAsync(key.Prepend(_prefix), member, order, flags);
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveAsync(key.Prepend(_prefix), member, flags);
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveAsync(key.Prepend(_prefix), members, flags);
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByRankAsync(key.Prepend(_prefix), start, stop, flags);
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByScoreAsync(key.Prepend(_prefix), start, stop, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByValueAsync(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetScoreAsync(key.Prepend(_prefix), member, flags);
        }

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringAppendAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitCountAsync(key.Prepend(_prefix), start, end, flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(),
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitOperationAsync(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.StringBitOperationAsync(operation, destination.Prepend(_prefix), keys, flags);
        }

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitPositionAsync(key.Prepend(_prefix), bit, start, end, flags);
        }

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringDecrementAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringDecrementAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetAsync(key.Prepend(_prefix), flags);
        }

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.StringGetAsync(keys, flags);
        }

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetBitAsync(key.Prepend(_prefix), offset, flags);
        }

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetRangeAsync(key.Prepend(_prefix), start, end, flags);
        }

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetSetAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetWithExpiryAsync(key.Prepend(_prefix), flags);
        }

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringIncrementAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringIncrementAsync(key.Prepend(_prefix), value, flags);
        }

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringLengthAsync(key.Prepend(_prefix), flags);
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSetAsync(key.Prepend(_prefix), value, expiry, when, flags);
        }

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var val = values[i];
                values[i] = new KeyValuePair<RedisKey, RedisValue>(val.Key.Prepend(_prefix), val.Value);
            }
            return _impl.StringSetAsync(values, when, flags);
        }

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSetBitAsync(key.Prepend(_prefix), offset, bit, flags);
        }

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSetRangeAsync(key.Prepend(_prefix), offset, value, flags);
        }

        public IBatch CreateBatch(object asyncState = null)
        {
            return _impl.CreateBatch(asyncState);
        }

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0,
            MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            _impl.KeyMigrate(key.Prepend(_prefix), toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public ITransaction CreateTransaction(object asyncState = null)
        {
            return _impl.CreateTransaction(asyncState);
        }

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.DebugObject(key.Prepend(_prefix), flags);
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDecrement(key.Prepend(_prefix), hashField, value, flags);
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDecrement(key.Prepend(_prefix), hashField, value, flags);
        }

        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDelete(key.Prepend(_prefix), hashField, flags);
        }

        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashDelete(key.Prepend(_prefix), hashFields, flags);
        }

        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashExists(key.Prepend(_prefix), hashField, flags);
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGet(key.Prepend(_prefix), hashField, flags);
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGet(key.Prepend(_prefix), hashFields, flags);
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashGetAll(key.Prepend(_prefix), flags);
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashIncrement(key.Prepend(_prefix), hashField, value, flags);
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashIncrement(key.Prepend(_prefix), hashField, value, flags);
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashKeys(key.Prepend(_prefix), flags);
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashLength(key.Prepend(_prefix), flags);
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            return _impl.HashScan(key.Prepend(_prefix), pattern, pageSize, flags);
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
            int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashScan(key.Prepend(_prefix), pattern, pageSize, cursor, pageOffset, flags);
        }

        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            _impl.HashSet(key.Prepend(_prefix), hashFields, flags);
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashSet(key.Prepend(_prefix), hashField, value, when, flags);
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HashValues(key.Prepend(_prefix), flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogAdd(key.Prepend(_prefix), value, flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogAdd(key.Prepend(_prefix), values, flags);
        }

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.HyperLogLogLength(key.Prepend(_prefix), flags);
        }

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.HyperLogLogLength(keys, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            _impl.HyperLogLogMerge(destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < sourceKeys.Length; i++)
            {
                sourceKeys[i] = sourceKeys[i].Prepend(_prefix);
            }
            _impl.HyperLogLogMerge(destination.Prepend(_prefix), sourceKeys, flags);
        }

        public EndPoint IdentifyEndpoint(RedisKey key = new RedisKey(), CommandFlags flags = CommandFlags.None)
        {
            return _impl.IdentifyEndpoint(key.Prepend(_prefix), flags);
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyDelete(key.Prepend(_prefix), flags);
        }

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.KeyDelete(keys, flags);
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyDump(key.Prepend(_prefix), flags);
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExists(key.Prepend(_prefix), flags);
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExpire(key.Prepend(_prefix), expiry, flags);
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyExpire(key.Prepend(_prefix), expiry, flags);
        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyMove(key.Prepend(_prefix), database, flags);
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyPersist(key.Prepend(_prefix), flags);
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyRandom(flags);
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyRename(key.Prepend(_prefix), newKey.Prepend(_prefix), when, flags);
        }

        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            _impl.KeyRestore(key.Prepend(_prefix), value, expiry, flags);
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyTimeToLive(key.Prepend(_prefix), flags);
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.KeyType(key.Prepend(_prefix), flags);
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListGetByIndex(key.Prepend(_prefix), index, flags);
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListInsertAfter(key.Prepend(_prefix), pivot, value, flags);
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListInsertBefore(key.Prepend(_prefix), pivot, value, flags);
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPop(key.Prepend(_prefix), flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPush(key.Prepend(_prefix), value, when, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLeftPush(key.Prepend(_prefix), values, flags);
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListLength(key.Prepend(_prefix), flags);
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRange(key.Prepend(_prefix), start, stop, flags);
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRemove(key.Prepend(_prefix), value, count, flags);
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPop(key.Prepend(_prefix), flags);
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPopLeftPush(source.Prepend(_prefix), destination.Prepend(_prefix), flags);
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPush(key.Prepend(_prefix), value, when, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.ListRightPush(key.Prepend(_prefix), values, flags);
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            _impl.ListSetByIndex(key.Prepend(_prefix), index, value, flags);
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            _impl.ListTrim(key.Prepend(_prefix), start, stop, flags);
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockExtend(key.Prepend(_prefix), value, expiry, flags);
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockQuery(key.Prepend(_prefix), flags);
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockRelease(key.Prepend(_prefix), value, flags);
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return _impl.LockTake(key.Prepend(_prefix), value, expiry, flags);
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            return _impl.Publish(channel, message, flags);
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (keys != null)
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = keys[i].Prepend(_prefix);
                }
            return _impl.ScriptEvaluate(script, keys, values, flags);
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (keys != null)
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = keys[i].Prepend(_prefix);
                }
            return _impl.ScriptEvaluate(hash, keys, values, flags);
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetAdd(key.Prepend(_prefix), value, flags);
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetAdd(key.Prepend(_prefix), values, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetCombine(operation, first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SetCombine(operation, keys, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetCombineAndStore(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SetCombineAndStore(operation, destination.Prepend(_prefix), keys, flags);
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetContains(key.Prepend(_prefix), value, flags);
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetLength(key.Prepend(_prefix), flags);
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetMembers(key.Prepend(_prefix), flags);
        }

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetMove(source.Prepend(_prefix), destination.Prepend(_prefix), value, flags);
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetPop(key.Prepend(_prefix), flags);
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRandomMember(key.Prepend(_prefix), flags);
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRandomMembers(key.Prepend(_prefix), count, flags);
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRemove(key.Prepend(_prefix), value, flags);
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetRemove(key.Prepend(_prefix), values, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            return _impl.SetScan(key.Prepend(_prefix), pattern, pageSize, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
            int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SetScan(key.Prepend(_prefix), pattern, pageSize, cursor, pageOffset, flags);
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric,
            RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return _impl.Sort(key.Prepend(_prefix), skip, take, order, sortType, @by, get, flags);
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending,
            SortType sortType = SortType.Numeric, RedisValue @by = new RedisValue(), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortAndStore(destination.Prepend(_prefix), key.Prepend(_prefix), skip, take, order, sortType, @by, get, flags);
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetAdd(key.Prepend(_prefix), member, score, flags);
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetAdd(key.Prepend(_prefix), values, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetCombineAndStore(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), aggregate, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null,
            Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.SortedSetCombineAndStore(operation, destination.Prepend(_prefix), keys, weights, aggregate, flags);
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetDecrement(key.Prepend(_prefix), member, value, flags);
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetIncrement(key.Prepend(_prefix), member, value, flags);
        }

        public long SortedSetLength(RedisKey key, double min = double.PositiveInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetLength(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetLengthByValue(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByRank(key.Prepend(_prefix), start, stop, order, flags);
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByRankWithScores(key.Prepend(_prefix), start, stop, order, flags);
        }

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByScore(key.Prepend(_prefix), start, stop, exclude, order, skip, take, flags);
        }

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByScoreWithScores(key.Prepend(_prefix), start, stop, exclude, order, skip, take, flags);
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(),
            Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRangeByValue(key.Prepend(_prefix), min, max, exclude, skip, take, flags);
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRank(key.Prepend(_prefix), member, order, flags);
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemove(key.Prepend(_prefix), member, flags);
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemove(key.Prepend(_prefix), members, flags);
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByRank(key.Prepend(_prefix), start, stop, flags);
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByScore(key.Prepend(_prefix), start, stop, exclude, flags);
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None,
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetRemoveRangeByValue(key.Prepend(_prefix), min, max, exclude, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            return _impl.SortedSetScan(key.Prepend(_prefix), pattern, pageSize, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = new RedisValue(), int pageSize = 10, long cursor = 0,
            int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetScan(key.Prepend(_prefix), pattern, pageSize, cursor, pageOffset, flags);
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _impl.SortedSetScore(key.Prepend(_prefix), member, flags);
        }

        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringAppend(key.Prepend(_prefix), value, flags);
        }

        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitCount(key.Prepend(_prefix), start, end, flags);
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = new RedisKey(),
            CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitOperation(operation, destination.Prepend(_prefix), first.Prepend(_prefix), second.Prepend(_prefix), flags);
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.StringBitOperation(operation, destination.Prepend(_prefix), keys, flags);
        }

        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringBitPosition(key.Prepend(_prefix), bit, start, end, flags);
        }

        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringDecrement(key.Prepend(_prefix), value, flags);
        }

        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringDecrement(key.Prepend(_prefix), value, flags);
        }

        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGet(key.Prepend(_prefix), flags);
        }

        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Prepend(_prefix);
            }
            return _impl.StringGet(keys, flags);
        }

        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetBit(key.Prepend(_prefix), offset, flags);
        }

        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetRange(key.Prepend(_prefix), start, end, flags);
        }

        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetSet(key.Prepend(_prefix), value, flags);
        }

        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringGetWithExpiry(key.Prepend(_prefix), flags);
        }

        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringIncrement(key.Prepend(_prefix), value, flags);
        }

        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringIncrement(key.Prepend(_prefix), value, flags);
        }

        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringLength(key.Prepend(_prefix), flags);
        }

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSet(key.Prepend(_prefix), value, expiry, when, flags);
        }

        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var val = values[i];
                values[i] = new KeyValuePair<RedisKey, RedisValue>(val.Key.Prepend(_prefix), val.Value);
            }
            return _impl.StringSet(values, when, flags);
        }

        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSetBit(key.Prepend(_prefix), offset, bit, flags);
        }

        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return _impl.StringSetRange(key.Prepend(_prefix), offset, value, flags);
        }

        public int Database
        {
            get { return _impl.Database; }
        }

        public IDatabaseArea CreateSubArea(RedisKey area)
        {
            return new DatabaseWrapper(_impl, area);
        }

        public IDatabase Parent { get { return _impl; } }
        public CarolineScriptsRepo Scripts { get { return _scripts; } }
        public RedisKey KeyPrefix { get { return _prefix; } }
    }
}
