using System;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IEntityTable<TEntity>
    {
        Task<TEntity> Get(TEntity id);
        Task<bool> Set(TEntity entity, TimeSpan? expiry = null);
        Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null);
        Task<bool> Delete(TEntity entity);
    }
}
