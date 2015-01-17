using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, new()
        where TId : IEquatable<TId>
    {
        protected Repository(IGoldRushDbContext context, DbSet<TEntity> set)
        {
            Context = context;
            EfContext = context.GetContext();
            Set = set;
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            var addedEntity = Set.Add(entity);
            await EfContext.SaveChangesAsync();
            return addedEntity;
        }

        public void Remove(TEntity entity)
        {
            EfContext.Entry(entity).State = EntityState.Deleted;
        }

        public abstract void Remove(TId id);

        public virtual void Update(TEntity entity)
        {
            EfContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<IEnumerable<TEntity>> Get()
        {
            return await (from e in Set select e).ToListAsync();
        }

        public abstract Task<TEntity> Get(TId id);

        protected DbSet<TEntity> Set { get; private set; }

        protected IGoldRushDbContext Context { get; private set; }
        protected DbContext EfContext { get; private set; }
    }

    public abstract class Repository<TEntity> : Repository<TEntity, int>
        where TEntity : class, new()
    {
        protected Repository(IGoldRushDbContext context, DbSet<TEntity> set) : base(context, set)
        {
        }
    }
}
