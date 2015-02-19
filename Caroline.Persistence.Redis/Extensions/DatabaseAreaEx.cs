using System;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DatabaseAreaEx
    {
        public static IDatabaseArea CreateSubArea(this IDatabaseArea area, long id)
        {
            return area.CreateSubArea(VarintBitConverter.GetVarintBytes(id));
        }

        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, ILongTable idIncrementDb, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier, TimeSpan? defaultExpiry = null)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "Id must be greater than 0.");
            var db = area.CreateSubArea(id);
            return new AutoKeyRedisEntityTable<TEntity>(db, idIncrementDb, id, serializer, identifier, defaultExpiry);
        }

        public static IAutoKeyEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, ILongTable idIncrementDb, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<long>
        {
            return area.Set(id, idIncrementDb, Objects<TEntity>.Serializer, Objects<TEntity, long>.Identifier, defaultExpiry);
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, ISerializer<TEntity> serializer, IIdentifier<TEntity, byte[]> identifier, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisEntityTable<TEntity>(db, serializer, identifier, defaultExpiry);
        } 

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, TimeSpan? defaultExpiry = null)
            where TEntity : IIdentifiableEntity<byte[]>
        {
            return area.Set(id, Objects<TEntity>.Serializer, Objects<TEntity, byte[]>.Identifier, defaultExpiry);
        }

        public static ILongTable SetLong(this IDatabaseArea area, long id, TimeSpan? defaultExpiry = null)
        {
            var db = area.CreateSubArea(id);
            return new RedisLongTable(db, defaultExpiry);
        }

        public static IStringTable SetString(this IDatabaseArea area, long id)
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