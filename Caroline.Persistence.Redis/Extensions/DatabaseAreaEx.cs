using System;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DatabaseAreaEx
    {
        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ILongTable idIncrementDb, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new AutoKeyRedisEntityTable<TEntity>(db, idIncrementDb, id, serializer, identifier, defaultExpiry);
        }

        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ILongTable idIncrementDb, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<long>
        {
            return Set(area, id, idIncrementDb, Objects<TEntity>.Serializer, Objects<TEntity, long>.Identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ISerializer<TEntity> serializer, IIdentifier<TEntity, RedisKey> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisEntityTable<TEntity>(db, serializer, identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<RedisKey>
        {
            return Set(area, id, Objects<TEntity>.Serializer, Objects<TEntity, RedisKey>.Identifier, defaultExpiry);
        }

        public static IPessimisticLockTable<TEntity> SetLock<TEntity>(this IDatabaseArea area, RedisKey id, IIdentifier<TEntity, RedisKey> identifier, TimeSpan expiry)
        {
            var db = area.CreateSubArea(id);
            return new RedisPessimisticLockTable<TEntity>(db, Objects<TEntity>.Serializer, identifier, expiry);
        }

        public static IPessimisticLockTable<TEntity> SetLock<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan expiry)
            where TEntity : IIdentifiableEntity<RedisKey>
        {
            var db = area.CreateSubArea(id);
            return new RedisPessimisticLockTable<TEntity>(db, Objects<TEntity>.Serializer, Objects<TEntity, RedisKey>.Identifier, expiry);
        }

        public static IPessimisticLockTable<TEntity> SetLockLong<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan expiry)
            where TEntity : IIdentifiableEntity<long>
        {
            var db = area.CreateSubArea(id);
            return SetLock(db, id, new LongIdentifier<TEntity>(Objects<TEntity, long>.Identifier), expiry);
        }

        public static ILongTable SetLong(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisLongTable(db, defaultExpiry);
        }

        public static IStringTable SetString(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisStringTable(db, defaultExpiry);
        }

        public static IEntityListTable<TEntity> SetList<TEntity>(this IDatabaseArea area, RedisKey id,
            ISerializer<TEntity> serializer, IIdentifier<TEntity, RedisKey> identifier)
        {
            // there is no defaultExpiry, as redis creates lists randomly during operations such as LPUSH,
            // and we have to check key existence before every type of operation, then set expiry
            // this is doable, but requires many small scripts
            var db = area.CreateSubArea(id);
            return new RedisEntityListTable<TEntity>(db, serializer, identifier);
        }

        public static IEntityListTable<TEntity> SetList<TEntity>(this IDatabaseArea area, RedisKey id)
            where TEntity : IIdentifiableEntity<RedisKey>
        {
            // see other comment on the other method overload about defaultExpiry
            return SetList(area, id, Objects<TEntity>.Serializer, Objects<TEntity, RedisKey>.Identifier);
        }

        static class Objects<TEntity>
        {
            public static readonly ISerializer<TEntity> Serializer = new ProtoBufSerializer<TEntity>();
        }

        static class Objects<TEntity, TId> where TEntity : IIdentifiableEntity<TId>
        {
            public static readonly IIdentifier<TEntity, TId> Identifier = new EntityIdentifier<TEntity, TId>();
        }
    }
}