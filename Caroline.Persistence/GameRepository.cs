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
    }
}
