using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using MongoDB.Driver;
using Nito.AsyncEx;
using DomainOrder = Caroline.Domain.Models.Order;

namespace Caroline.Domain
{
    class MarketPlace
    {
        CarolineRedisDb _redis = CarolineRedisDb.Create();
        static CarolineMongoDb _mongo;
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        MarketPlace() { }

        public static async Task<MarketPlace> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
                if (_mongo == null)
                    _mongo = await CarolineMongoDb.CreateAsync();
            return new MarketPlace();
        }

        public async Task Transact(DomainOrder order)
        {
            var isCounterPartSelling = !order.IsSelling;
            var orderSearchPredicate = order.IsSelling 
                ? ComparisonPredicate.GreaterThan 
                : ComparisonPredicate.LessThan;
            var freshOrder = new StaleOrder
            {
                Quantity = order.Quantity,
                UnfulfilledQuantity = order.Quantity,
                IsSelling = order.IsSelling,
                ItemId = order.ItemId,
                UnitValue = order.UnitValue
            };

            while (order.Quantity > 0)
            {
                var staleOrder = await GetOrder(order.ItemId, isCounterPartSelling, order.UnitValue, orderSearchPredicate);
                if (staleOrder == null)
                    break;

                var unitsTransacted = Math.Min(staleOrder.Quantity, order.Quantity);
                freshOrder.UnfulfilledQuantity -= unitsTransacted;
                // TODO: check uses of unfulfilledQuantity after its been changed from fulfilledQuantity
                // TODO: code switching from buy/sell orders to fresh/stale orders
                staleOrder.UnfulfilledQuantity -= unitsTransacted;

                // delete or update stale order
                if (staleOrder.Quantity == 0)
                    // remove buy order from active marketplace
                    // TODO: !!! process return values of these calls to check for races, retry
                    await DeleteStaleOrder(staleOrder.Id);
                else
                    await UpdateStaleOrder(staleOrder);

                // stale order advantage, give buyer the difference in price
                var differenceToRefund = (staleOrder.UnitWorth - order.UnitValue) * unitsTransacted;
                GiveBuyerMoney(differenceToRefund);

            }

            // update sell order
            if (order.Quantity == 0)
                ; // delete order.. but we never commited it to the db so what ever
            else
                UpdateStaleOrder(order);
        }

        async Task<StaleOrder> GetOrder(long itemId, bool isSelling, long unitValue, ComparisonPredicate unitValueComparison)
        {
            Expression<Func<StaleOrder, bool>> predicate;
            switch (unitValueComparison)
            {
                case ComparisonPredicate.GreaterThan:
                    predicate = o => o.ItemId == itemId
                        && o.IsSelling == isSelling
                        && o.UnfulfilledQuantity != 0
                        && o.UnitValue >= unitValue;
                    break;
                case ComparisonPredicate.LessThan:
                    predicate = o => o.ItemId == itemId
                        && o.IsSelling == isSelling
                        && o.UnfulfilledQuantity != 0
                        && o.UnitValue <= unitValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitValueComparison");
            }

            var result = await _mongo.Orders.FindAsync(predicate, new FindOptions<StaleOrder, StaleOrder> { Limit = 1 });
            var list = await result.ToListAsync();
            return list.SingleOrDefault();
        }

        async Task InsertStaleOrder(StaleOrder staleOrder)
        {
            if (staleOrder.Version != 0)
            {
                Log.Warn("Inserting a new order with a version != 0");
                staleOrder.Version = 0;
            }
            if (staleOrder.Id != 0)
            {
                Log.Warn("Inserting a new order with an id != 0");
                staleOrder.Id = 0;
            }

            var result = await _mongo.Orders.UpdateOneAsync(o => true, new ObjectUpdateDefinition<StaleOrder>(staleOrder),
                new UpdateOptions { IsUpsert = true });
            staleOrder.Id = result.UpsertedId.AsInt64;
        }

        async Task<VersionedUpdateResult> UpdateStaleOrder(StaleOrder staleOrder)
        {
            var oldVersion = staleOrder.Version++;
            var result = await _mongo.Orders.UpdateOneAsync(o => o.Id == staleOrder.Id && o.Version == oldVersion, new ObjectUpdateDefinition<StaleOrder>(staleOrder));
            if (result.ModifiedCount == 1)
                return VersionedUpdateResult.Success;

            var findResult = await _mongo.Orders.FindAsync(o => o.Id == staleOrder.Id);
            var list = await findResult.ToListAsync();
            if (list.Count == 1)
                return VersionedUpdateResult.WrongVersion;
            if (list.Count == 0)
                return VersionedUpdateResult.DoesNotExist;
            Log.Warn("Multiple Orders with the same Id " + staleOrder.Id);
            return VersionedUpdateResult.DoesNotExist;
        }

        async Task<bool> DeleteStaleOrder(long id)
        {
            var result = await _mongo.Orders.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount == 1;
        }

        Task SendMoney(long gameId, long amount)
        {
            throw new NotImplementedException();
        }

        Task SendItems(long itemId, long amount)
        {
            throw new NotImplementedException();
        }
    }

    internal enum VersionedUpdateResult
    {
        Success,
        WrongVersion,
        DoesNotExist
    }

    internal enum ComparisonPredicate
    {
        GreaterThan,
        LessThan
    }
}
