using System;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public interface IEntityTable<TEntity>
    {
        Task<bool> Set(TEntity entity, SetMode mode, TimeSpan? expiry = null);
        Task<TEntity> GetSet(TEntity entity, TimeSpan? expiry = null);
    }
}