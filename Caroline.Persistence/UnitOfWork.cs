using System;
using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly IGoldRushDbContext _context;

        public UnitOfWork(IGameRepository games, IUserRepository users, IGoldRushDbContext context)
        {
            if (games == null) throw new ArgumentNullException("games");
            if (users == null) throw new ArgumentNullException("users");
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
            Games = games;
            Users = users;
        }

        public IGameRepository Games { get; private set; }
        public IUserRepository Users { get; private set; }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}