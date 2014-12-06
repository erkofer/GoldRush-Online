using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    class UnitOfWork : IUnitOfWork
    {
        readonly GoldRushDbContext _context = new GoldRushDbContext();
        public IRepository<Game> Games { get; private set; }
        public IRepository<ApplicationUser> Users { get; private set; }
        public IRepository<IdentityRole> Roles { get; private set; }

        public void Dispose()
        {
            Games = new Repository<Game>(_context);
            Users = new Repository<ApplicationUser>(_context);
            Roles = new Repository<IdentityRole>(_context);
        }
    }
}