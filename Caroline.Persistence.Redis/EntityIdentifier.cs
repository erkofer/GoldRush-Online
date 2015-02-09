namespace Caroline.Persistence.Redis
{
    public class EntityIdentifier<TEntity, TId> 
        : IIdentifier<TEntity, TId> where TEntity : IIdentifiableEntity<TId>
    {
        public TId GetId(TEntity entity)
        {
            return entity.Id;
        }

        public void SetId(TEntity entity, TId value)
        {
            entity.Id = value;
        }
    }
}
