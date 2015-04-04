using System;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IEntityTable<TEntity, in TId>
    {
        Task<TEntity> Get(TId id);
        Task<bool> Set(TEntity entity, TimeSpan? expiry = null);
        Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null);
        Task<bool> Delete(TId id);
    }
}
