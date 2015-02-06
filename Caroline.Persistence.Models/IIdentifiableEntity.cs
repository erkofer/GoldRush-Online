namespace Caroline.Persistence.Models
{
    public interface IIdentifiableEntity<T>
    {
        T Id { get; set; }
    }
}
