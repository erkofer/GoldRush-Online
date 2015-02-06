using System.Threading.Tasks;

namespace Caroline.Persistence
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        GoldRushDbContext _Context;
        IGameRepository _games;
        IUserRepository _users;

        GoldRushDbContext Context
        {
            get { return _Context ?? (_Context = new GoldRushDbContext()); }
        }

        public IGameRepository Games
        {
            get { return _games ?? (_games = new SqlGameRepository(Context)); }
        }
        public IUserRepository Users
        {
            get { return _users ?? (_users = new SqlUserRepository(Context)); }
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