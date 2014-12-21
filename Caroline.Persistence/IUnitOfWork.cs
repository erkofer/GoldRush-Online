using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<IdentityRole> Roles { get; }
        IGameRepository Games { get; }
        IUserRepository Users { get; }
        Task SaveChangesAsync();
    }
}