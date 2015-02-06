namespace Caroline.Persistence
{
    public interface IIdentifier<in TEntity>
    {
        long GetId(TEntity entity);
        void SetId(TEntity entity, long value);
    }
}
