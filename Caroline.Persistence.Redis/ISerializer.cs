namespace Caroline.Persistence.Redis
{
    public interface ISerializer<TEntity>
    {
        byte[] Serialize(TEntity entity);
        TEntity Deserialize(byte[] data);
    }
}
