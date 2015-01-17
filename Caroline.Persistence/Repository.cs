using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IIdentifiableEntity<TId>, new()
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

        public void Remove(TId id)
        {
            var entity = from e in Set
                         where e.EntityId.Equals(id)
                         select e;
            EfContext.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void Update(TEntity entity)
        {
            EfContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<IEnumerable<TEntity>> Get()
        {
            return await (from e in Set select e).ToListAsync();
        }

        public virtual async Task<TEntity> Get(TId id)
        {
            return await (from e in Set
                          where e.EntityId.Equals(id)
                          select e).SingleAsync();
        }

        protected DbSet<TEntity> Set { get; private set; }

        protected IGoldRushDbContext Context { get; private set; }
        protected DbContext EfContext { get; private set; }
    }

    public abstract class Repository<TEntity> : Repository<TEntity, int>
        where TEntity : class, IIdentifiableEntity<int>, new()
    {
        protected Repository(IGoldRushDbContext context, DbSet<TEntity> set) : base(context, set)
        {
        }
    }
}
