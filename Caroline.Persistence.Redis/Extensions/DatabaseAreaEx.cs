using System;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DatabaseAreaEx
    {
        public static IDatabaseArea CreateSubArea(this IDatabaseArea area, RedisKey id)
        {
            return area.CreateSubArea(id);
        }

        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ILongTable idIncrementDb, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new AutoKeyRedisEntityTable<TEntity>(db, idIncrementDb, id, serializer, identifier, defaultExpiry);
        }

        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ILongTable idIncrementDb, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<long>
        {
            return area.Set(id, idIncrementDb, Objects<TEntity>.Serializer, Objects<TEntity, long>.Identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, ISerializer<TEntity> serializer, IIdentifier<TEntity, string> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisEntityTable<TEntity>(db, serializer, identifier, defaultExpiry);
        } 

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<string>
        {
            return area.Set(id, Objects<TEntity>.Serializer, Objects<TEntity, string>.Identifier, defaultExpiry);
        }

        public static ILongTable SetLong(this IDatabaseArea area, RedisKey id, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisLongTable(db, defaultExpiry);
        }

        public static IStringTable SetString(this IDatabaseArea area, RedisKey id)
        {
            var db = area.CreateSubArea(id);
            return new RedisStringTable(db);
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