using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public sealed class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(GoldRushDbContext context)
            :base(context, context.Set<Game>())
        {
        }
    }
}
