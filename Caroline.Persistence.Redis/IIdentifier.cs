namespace Caroline.Persistence.Redis
{
    public interface IIdentifier<in TEntity>
    {
        long GetId(TEntity entity);
        void SetId(TEntity entity, long value);
    }
}
