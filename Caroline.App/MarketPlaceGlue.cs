using System.Threading.Tasks;
using Caroline.Domain;
using Caroline.Domain.Models;
using Caroline.Persistence.Models;
using GoldRush.Market;
using MongoDB.Driver;

namespace Caroline.App
{
    class MarketPlaceGlue : IMarketPlace
    {
        private readonly MarketPlace _impl;

        public MarketPlaceGlue(MarketPlace impl)
        {
            _impl = impl;
        }

        public Task<StaleOrder> Transact(FreshOrder order)
        {
            return _impl.Transact(order);
        }

        public Task<StaleOrder> GetOrder(string id)
        {
            return _impl.GetOrder(id);
        }

        public Task<IAsyncCursor<StaleOrder>> GetOrders(long gameId)
        {
            return _impl.GetOrdersByGame(gameId);
        }

        public Task<long?> ClaimOrderContents(string id, ClaimField field)
        {
            return _impl.ClaimOrderContents(id, field);
        }
    }
}
