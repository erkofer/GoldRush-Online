using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IAsyncDisposable
    {
        Task DisposeAsync();
    }
}
