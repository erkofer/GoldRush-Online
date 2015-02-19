using System;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IAutoKeyEntityTable<TEntity>
    {
        Task<TEntity> Get(long id);
        Task<bool> Set(TEntity entity, SetMode mode, TimeSpan? expiry = null);
        Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null);

        Task<bool> Delete(long id);
    }
}