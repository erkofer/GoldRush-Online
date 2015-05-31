using System.Threading.Tasks;
using Caroline.Domain.Models;
using Caroline.Persistence.Models;
using MongoDB.Driver;

namespace GoldRush.Market
{
    public interface IMarketPlace
    {
        Task<StaleOrder> Transact(FreshOrder order);
        Task<IAsyncCursor<StaleOrder>> GetOrders(long gameId);
        Task<StaleOrder> GetOrder(string id);
        Task<long?> ClaimOrderContents(string id, ClaimField field);
    }
}
