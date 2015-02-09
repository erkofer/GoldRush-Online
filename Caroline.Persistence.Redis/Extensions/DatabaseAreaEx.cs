using System;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DatabaseAreaEx
    {
        public static IDatabaseArea CreateArea(this IDatabaseArea area, long id)
        {
            return area.CreateSubArea(VarintBitConverter.GetVarintBytes(id));
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, ILongTable idIncrementDb, ISerializer<TEntity> serializer, IIdentifier<TEntity, long> identifier)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "Id must be greater than 0.");
            var db = area.CreateArea(id);
            return new AutoKeyRedisEntityTable<TEntity>(db, idIncrementDb, id, serializer, identifier);
        }

        public static IEntityTable<TEntity> Set<TEntity>(this IDatabaseArea area, long id, ILongTable idIncrementDb)
            where TEntity : IIdentifiableEntity<long>
        {
            return area.Set(id, idIncrementDb, Objects<TEntity>.Serializer, Objects<TEntity, long>.Identifier);
        }

        public static ILongTable SetLong(this IDatabaseArea area, long id)
        {
            var db = area.CreateArea(id);
            return new RedisLongTable(db);
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