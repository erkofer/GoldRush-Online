namespace Caroline.Persistence
{
    public interface IIdentifiableEntity<T>
    {
        T EntityId { get; set; }
    }
    public interface IIdentifiableEntity : IIdentifiableEntity<int>
    {
    }
}
