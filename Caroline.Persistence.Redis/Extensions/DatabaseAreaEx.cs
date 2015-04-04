using System;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DatabaseAreaEx
    {
        public static IEntityTable<TEntity, TId> Set<TEntity, TId>(this IDatabaseArea area, RedisKey id, ISerializer<TEntity> serializer, ISerializer<TId> idSerializer, IIdentifier<TEntity, TId> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisEntityTable<TEntity, TId>(db, serializer, idSerializer, identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity, TId> Set<TEntity, TId>(this IDatabaseArea area, RedisKey id, ISerializer<TId> idSerializer, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<TId>
        {
            return Set(area, id, Objects<TEntity>.Serializer, idSerializer, Objects<TEntity, TId>.Identifier, defaultExpiry);
        }
        public static IEntityTable<TEntity, TId> Set<TEntity, TId>(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
            where TId : IIdentifiableEntity<string>, new()
            where TEntity : IIdentifiableEntity<TId>
        {
            
            return Set(area, id, Objects<TEntity>.Serializer, ObjectsString<TId>.StringSerializer, Objects<TEntity, TId>.Identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity, byte[]> SetByte<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<byte[]>
        {
            return Set(area, id, Objects<TEntity>.Serializer, Objects.ByteSerializer, Objects<TEntity, byte[]>.Identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity, long> SetLong<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<long>
        {
            return Set(area, id, Objects<TEntity>.Serializer, Objects.LongSerializer, Objects<TEntity, long>.Identifier, defaultExpiry);
        }

        public static IPessimisticLockTable<TId> Lock<TId>(this IDatabaseArea area, RedisKey id, ISerializer<TId> serializer, TimeSpan expiry)
        {
            var db = area.CreateSubArea(id);
            return new RedisPessimisticLockTable<TId>(db, serializer, expiry);
        }

        public static IPessimisticLockTable<TId> Lock<TId>(this IDatabaseArea area, RedisKey id, TimeSpan expiry)
        {
            var db = area.CreateSubArea(id);
            return new RedisPessimisticLockTable<TId>(db, Objects<TId>.Serializer, expiry);
        }

        public static IPessimisticLockTable<long> LockLong(this IDatabaseArea area, RedisKey id, TimeSpan expiry)
        {
            return Lock(area, id, Objects.LongSerializer, expiry);
        }

        public static IIdManager<TEntity> IdManager<TEntity>(this IDatabaseArea area, RedisKey id, IIdentifier<TEntity, long> identifier)
        {
            var db = area.CreateSubArea(id);
            return new RedisIdManager<TEntity>(db, identifier);
        }
        public static IIdManager<TEntity> IdManager<TEntity>(this IDatabaseArea area, RedisKey id)
            where TEntity : IIdentifiableEntity<long>
        {
            var db = area.CreateSubArea(id);
            return new RedisIdManager<TEntity>(db, Objects<TEntity, long>.Identifier);
        }

        public static IStringTable String(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisStringTable(db, defaultExpiry);
        }

        public static IEntityListTable<TEntity, TId> SetList<TEntity, TId>(this IDatabaseArea area, RedisKey id, ISerializer<TEntity> serializer, ISerializer<TId> keySerializer, IIdentifier<TEntity, TId> identifier)
        {
            // there is no defaultExpiry, as redis creates lists randomly during operations such as LPUSH,
            // and we have to check key existence before every type of operation, then set expiry
            // this is doable, but requires many small scripts
            var db = area.CreateSubArea(id);
            return new RedisEntityListTable<TEntity, TId>(db, serializer, keySerializer, identifier);
        }

        public static IEntityListTable<TEntity, TId> SetList<TEntity, TId>(this IDatabaseArea area, RedisKey id)
            where TEntity : IIdentifiableEntity<TId>
        {
            // see other comment on the other method overload about defaultExpiry
            return SetList(area, id, Objects<TEntity>.Serializer, Objects<TId>.Serializer, Objects<TEntity, TId>.Identifier);
        }

        static class Objects
        {
            public static readonly ISerializer<byte[]> ByteSerializer = new ByteSerializer();
            public static readonly ISerializer<long> LongSerializer = new LongSerializer();
            public static readonly ISerializer<string> StringSerializer = new StringSerializer();
        }

        static class Objects<TEntity>
        {
            public static readonly ISerializer<TEntity> Serializer = new ProtoBufSerializer<TEntity>();
        }

        static class ObjectsString<TEntity>
            where TEntity : IIdentifiableEntity<string>, new()
        {
            public static readonly ISerializer<TEntity> StringSerializer = new StringSerializer<TEntity>();
        }

        static class Objects<TEntity, TId> where TEntity : IIdentifiableEntity<TId>
        {
            public static readonly IIdentifier<TEntity, TId> Identifier = new EntityIdentifier<TEntity, TId>();
        }
    }
}