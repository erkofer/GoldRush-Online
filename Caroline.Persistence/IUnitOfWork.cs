using System;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Games { get; }
        IUserRepository Users { get; }
        Task SaveChangesAsync();
    }
}