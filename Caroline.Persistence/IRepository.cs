using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object id);
        void Update(TEntity entity);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
