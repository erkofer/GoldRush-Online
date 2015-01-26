using JetBrains.Annotations;

namespace Caroline.App.Models
{
    public interface ICompressable<T>
    {
        [CanBeNull]
        T Compress([NotNull]T oldObject);
    }
}
