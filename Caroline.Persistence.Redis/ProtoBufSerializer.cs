using System;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    class ProtoBufSerializer<TEntity> : ISerializer<TEntity>
    {
        // prefer castinr RedisValue to and from byte[],
        // as thats reduces allocations when using StackExchange.Redis
        public byte[] Serialize(TEntity entity)
        {
            return ProtoBufHelpers.SerializeToBytes(entity);
        }

        public TEntity Deserialize(byte[] data)
        {
            return ProtoBufHelpers.Deserialize<TEntity>(data);
        }
    }

    class StringSerializer : Serializer<string>
    {
        public StringSerializer()
            : base(s => (RedisKey)s, b => (RedisKey)b)
        {
        }
    }

    class StringSerializer<TEntity> : Serializer<TEntity>
        where TEntity : IIdentifiableEntity<string>, new()
    {
        public StringSerializer()
            : base(e => (RedisKey)e.Id, id => new TEntity { Id = (RedisKey)id })
        {
        }
    }

    class ByteSerializer : Serializer<byte[]>
    {
        public ByteSerializer()
            : base(bytes => bytes, bytes => bytes)
        {

        }
    }

    class LongSerializer : Serializer<long>
    {
        public LongSerializer()
            : base(l => (RedisKey)l.ToStringInvariant(), bytes => long.Parse((RedisKey)bytes))
        {

        }
    }

    public class Serializer<TEntity> : ISerializer<TEntity>
    {
        readonly Func<TEntity, byte[]> _serialize;
        readonly Func<byte[], TEntity> _deserialize;

        public Serializer(Func<TEntity, byte[]> serialize, Func<byte[], TEntity> deserialize)
        {
            _serialize = serialize;
            _deserialize = deserialize;
        }

        public byte[] Serialize(TEntity entity)
        {
            return _serialize(entity);
        }

        public TEntity Deserialize(byte[] data)
        {
            return _deserialize(data);
        }
    }
}
