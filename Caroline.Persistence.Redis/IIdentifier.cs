namespace Caroline.Persistence.Redis
{
    public interface IIdentifier<in TEntity, TId>
    {
        TId GetId(TEntity entity);
        void SetId(TEntity entity, TId value);
    }
}
