using System;

namespace Caroline.Dal
{
    public class UnitOfWork : IDisposable
    {
        readonly ApplicationDbContext _context = ApplicationDbContext.Create();
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