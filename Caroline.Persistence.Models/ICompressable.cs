using JetBrains.Annotations;

namespace Caroline.Persistence.Models
{
    public interface ICompressable<T>
    {
        [CanBeNull]
        T Compress([NotNull]T oldObject);
    }
}
