namespace Caroline.Persistence.Redis
{
    public interface IIdentifiableEntity<T>
    {
        T Id { get; set; }
    }
}
