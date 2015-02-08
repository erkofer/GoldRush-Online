namespace Caroline.Persistence.Redis
{
    public enum SetMode
    {
        /// <summary>
        /// Adds the entity, overwrites the entities Id with the one assigned by redis.
        /// </summary>
        Add,
        /// <summary>
        /// Overwrites an existing entity with matching Ids.
        /// </summary>
        Overwrite,
    }
}
