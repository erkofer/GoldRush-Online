using System;
using System.Globalization;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class LongIdentifier<TEntity> : Identifier<TEntity, RedisKey>
    {
        public LongIdentifier(IIdentifier<TEntity, long> identifier)
            : base(ent => identifier.GetId(ent).ToStringInvariant(), (ent, id) => identifier.SetId(ent, long.Parse(id, CultureInfo.InvariantCulture)))
        {

        }
    }

    public class Identifier<TEntity, TId> : IIdentifier<TEntity, TId>
    {
        readonly Func<TEntity, TId> _get;
        readonly Action<TEntity, TId> _set;

        public Identifier(Func<TEntity, TId> get, Action<TEntity, TId> set)
        {
            _get = get;
            _set = set;
        }

        public TId GetId(TEntity entity)
        {
            return _get(entity);
        }

        public void SetId(TEntity entity, TId value)
        {
            _set(entity, value);
        }
    }

    public class EntityIdentifier<TEntity, TId>
        : IIdentifier<TEntity, TId> where TEntity : IIdentifiableEntity<TId>
    {
        public TId GetId(TEntity entity)
        {
            return entity.Id;
        }

        public void SetId(TEntity entity, TId value)
        {
            entity.Id = value;
        }
    }
}
