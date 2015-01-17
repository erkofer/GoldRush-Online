using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public sealed class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(GoldRushDbContext context)
            : base(context, context.Set<Game>())
        {
        }

        public async Task<Game> GetByUserIdAsync(string userId)
        {
            var query = from e in Context.Users
                        where e.Id == userId
                        select e.Game;

            return await query.SingleOrDefaultAsync();
        }

        public async Task<Game> AddByUserIdAsync(string userId, Game game)
        {
            var userQuery = from e in Context.Users
                            where e.Id == userId
                            select e;
            var user = await userQuery.SingleOrDefaultAsync();
            if (user == null)
                return null;

            Set.Add(game);
            // link the game to the user
            user.Game = game;
            Context.Entry(user).State = EntityState.Modified;

            await Context.SaveChangesAsync();
            return game;
        }

        public override void Remove(int id)
        {
            var entity = from e in Set
                         where e.Id == id
                         select e;
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public override async Task<Game> Get(int id)
        {
            return await (from e in Set
                          where e.Id == id
                          select e).SingleOrDefaultAsync();
        }
    }
}
