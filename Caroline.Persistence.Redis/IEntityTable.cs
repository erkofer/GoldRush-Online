using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public interface IEntityTable<TEntity, in TId>
    {
        Task<TEntity> Get(TId id);
        Task<bool> Set(TEntity entity, TimeSpan? expiry = null, When when = When.Always);
        Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null);
        Task<bool> Delete(TId id);
    }
}
