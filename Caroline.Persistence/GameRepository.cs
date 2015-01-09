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

        public async Task<Game> GetByUseridAsync(string userId)
        {
            var query = from e in Context.Users
                        where e.Id == userId
                        select e.Game;

            return await query.SingleAsync();
        }

        public async Task<Game> AddByUserIdAsync(string userId, Game game)
        {
            var addedEntity = Set.Add(game);

            var userQuery = from e in Context.Users
                            where e.Id == userId
                            select e;
            var user = await userQuery.SingleAsync();

            // link the game to the user
            user.GameId = game.Id;
            Context.Entry(user).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return addedEntity;
        }
    }
}
