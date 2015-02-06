namespace Caroline.Persistence
{
    class ProtoBufSerializer<TEntity> : ISerializer<TEntity>
    {
        public byte[] Serialize(TEntity entity)
        {
            return ProtoBufHelpers.SerializeToBytes(entity);
        }

        public TEntity Deserialize(byte[] data)
        {
            return ProtoBufHelpers.Deserialize<TEntity>(data);
        }
    }
}
