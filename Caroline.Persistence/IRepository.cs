using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public interface IRepository<TEntity, in TId>
    {
        Task<IEnumerable<TEntity>> Get();
        Task<TEntity> Get(TId id);
        Task<TEntity> Add(TEntity entity);
        void Remove(TEntity entity);
        void Remove(TId id);
        void Update(TEntity entity);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class
    {
    }
}
