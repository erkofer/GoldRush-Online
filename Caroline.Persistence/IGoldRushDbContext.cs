using System.Data.Entity;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public interface IGoldRushDbContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<Game> Games { get;  }
        DbSet<IdentityRole> Roles { get;  }
        DbContext GetContext();
        Task SaveChangesAsync();
        void Dispose();
    }
}