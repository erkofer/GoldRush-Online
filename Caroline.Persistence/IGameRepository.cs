using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<Game> GetByUserIdAsync(string userId);
        Task<Game> AddByUserIdAsync(string userId, Game game);
    }
}