using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        GoldRushDbContext _Context;
        DbSet<IdentityRole> _roles;
        IGameRepository _games;
        IUserRepository _users;

        GoldRushDbContext Context
        {
            get { return _Context ?? (_Context = new GoldRushDbContext()); }
        }

        public DbSet<IdentityRole> Roles
        {
            get { return _roles ?? (_roles = Context.Set<IdentityRole>()); }
        }

        public IGameRepository Games
        {
            get { return _games ?? (_games = new SqlGameSqlRepository(Context)); }
        }
        public IUserRepository Users
        {
            get { return _users ?? (_users = new SqlUserSqlRepository(Context)); }
        }

        public async Task SaveChangesAsync()
        {
            if (_Context != null)
                await _Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_Context != null)
                _Context.Dispose();
        }
    }
}