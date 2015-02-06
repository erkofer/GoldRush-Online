using Caroline.Persistence.Models;
using ProtoBuf;

namespace Caroline.Persistence.Extensions
{
    public static class RedisDbEx
    {
        public static IEntityTable<TEntity> Set<TEntity>(this RedisDb db, long id) 
            where TEntity : IIdentifiableEntity<long>
        {
            return db.Set(id, Objects<TEntity>.Serializer, Objects<TEntity>.Identifier);
        }

        static class Objects<TEntity> where TEntity : IIdentifiableEntity<long>
        {
            public static readonly ISerializer<TEntity> Serializer = new ProtoBufSerializer<TEntity>();
            public static readonly IIdentifier<TEntity> Identifier = new EntityIdentifier<TEntity>();
        }
    }
}
