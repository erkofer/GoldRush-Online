using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    class UnitOfWork : IUnitOfWork
    {
        GoldRushDbContext _context;
        IRepository<IdentityRole> _roles;
        IRepository<Game> _games;
        IRepository<ApplicationUser> _users;

        GoldRushDbContext Context => _context ?? (_context = new GoldRushDbContext());

        public IRepository<IdentityRole> Roles => _roles ?? (_roles = new Repository<IdentityRole>(Context));
        public IRepository<Game> Games => _games ?? (_games = new Repository<Game>(Context));
        public IRepository<ApplicationUser> Users => _users ?? (_users = new Repository<ApplicationUser>(Context));

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}