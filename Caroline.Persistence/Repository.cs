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
        protected Repository(GoldRushDbContext context, DbSet<TEntity> set)
        {
            Context = context;
            Set = set;
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            var addedEntity = Set.Add(entity);
            await Context.SaveChangesAsync();
            return addedEntity;
        }

        public void Remove(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public void Remove(TId id)
        {
            var entity = from e in Set
                         where e.EntityId.Equals(id)
                         select e;
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
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

        protected DbSet<TEntity> Set { get; set; }

        protected GoldRushDbContext Context { get; set; }
    }

    public abstract class Repository<TEntity> : Repository<TEntity, int>
        where TEntity : class, IIdentifiableEntity<int>, new()
    {
        protected Repository(GoldRushDbContext context, DbSet<TEntity> set) : base(context, set)
        {
        }
    }
}
