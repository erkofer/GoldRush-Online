using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public class EntityIdentifier<TEntity> 
        : IIdentifier<TEntity> where TEntity : IIdentifiableEntity<long>
    {
        public long GetId(TEntity entity)
        {
            return entity.Id;
        }

        public void SetId(TEntity entity, long value)
        {
            entity.Id = value;
        }
    }
}
