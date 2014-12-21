using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<Game> GetByUseridAsync(string userId);
    }
}