using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caroline.Domain.Models;
using Caroline.Persistence;
using Caroline.Persistence.Extensions;
using Caroline.Persistence.Models;
using MongoDB.Driver;
using Nito.AsyncEx;
using DomainOrder = Caroline.Domain.Models.Order;

namespace Caroline.Domain
{
    class MarketPlace
    {
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

        public async Task<StaleOrder> Transact(DomainOrder order)
        {
            var isStaleOrderSelling = !order.IsSelling;
            var orderSearchPredicate = order.IsSelling
                ? ComparisonPredicate.GreaterThan
                : ComparisonPredicate.LessThan;

            var freshOrder = BuildStaleOrder(order);

            while (freshOrder.UnfulfilledQuantity > 0)
            {
                var staleOrder = await GetOrder(order.ItemId, isStaleOrderSelling, order.UnitValue, orderSearchPredicate);
                if (staleOrder == null)
                    break;

                var sellOrder = isStaleOrderSelling ? staleOrder : freshOrder;
                var buyOrder = isStaleOrderSelling ? freshOrder : staleOrder;

                var unitsTransacted = Math.Min(staleOrder.UnfulfilledQuantity, freshOrder.UnfulfilledQuantity);
                freshOrder.UnfulfilledQuantity -= unitsTransacted;
                staleOrder.UnfulfilledQuantity -= unitsTransacted;

                // exchange money and items
                var moneyExchanged = staleOrder.UnitValue * unitsTransacted;
                sellOrder.TotalMoneyRecieved += moneyExchanged;
                sellOrder.UnclaimedMoneyRecieved += moneyExchanged;
                buyOrder.TotalItemsRecieved += unitsTransacted;
                buyOrder.UnclaimedItemsRecieved += unitsTransacted;

                // stale order advantage, give stale order the difference in price
                var differenceToRefund = MathHelpers.Difference(staleOrder.UnitValue, freshOrder.UnitValue) * unitsTransacted;
                staleOrder.TotalMoneyRecieved += differenceToRefund;

                var result = await UpdateStaleOrder(staleOrder);
                switch (result)
                {
                    case VersionedUpdateResult.Success:
                        continue;
                    case VersionedUpdateResult.WrongVersion:
                        // reset freshOrder and try again
                        // staleOrder wasn't sent to the DB, but freshOrder has been modified
                        ResetOrder(freshOrder, unitsTransacted, moneyExchanged);
                        break;
                    case VersionedUpdateResult.DoesNotExist:
                        Log.Warn("Attempted to update StaleOrder with id that does not exist in DB.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await InsertOrder(freshOrder);
            return freshOrder;
        }

        public Task<StaleOrder> GetOrder(long id)
        {
            return _mongo.Orders.SingleOrDefault(o => o.Id == id);
        }

        public Task<IAsyncCursor<StaleOrder>> GetOrders(long gameId)
        {
            return _mongo.Orders.FindAsync(o => o.GameId == gameId);
        }

        public async Task<OrderClaimResult> ClaimOrderContents(long id, ClaimField field)
        {
            var order = await _mongo.Orders.SingleOrDefault(o => o.Id == id);
            if (order == null)
                return new OrderClaimResult { Status = VersionedUpdateResult.DoesNotExist };
            long numClaimed;
            switch (field)
            {
                case ClaimField.Items:
                    numClaimed = order.UnclaimedItemsRecieved;
                    order.UnclaimedItemsRecieved = 0;
                    break;
                case ClaimField.Money:
                    numClaimed = order.UnclaimedMoneyRecieved;
                    order.UnclaimedMoneyRecieved = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("field");
            }

            var updateResult = await UpdateStaleOrder(order);
            switch (updateResult)
            {
                case VersionedUpdateResult.Success:
                    return new OrderClaimResult { NumItems = numClaimed, Status = VersionedUpdateResult.Success };
                case VersionedUpdateResult.WrongVersion:
                    return new OrderClaimResult { Status = VersionedUpdateResult.WrongVersion };
                case VersionedUpdateResult.DoesNotExist:
                    return new OrderClaimResult { Status = VersionedUpdateResult.DoesNotExist };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static StaleOrder BuildStaleOrder(DomainOrder order)
        {
            return new StaleOrder
            {
                ItemId = order.ItemId,
                GameId = order.GameId,
                UnitValue = order.UnitValue,
                Quantity = order.Quantity,
                UnfulfilledQuantity = order.Quantity,
                IsSelling = order.IsSelling,
                Version = 1
            };
        }

        static void ResetOrder(StaleOrder order, long unitsTransacted, long moneyExchanged)
        {
            order.UnfulfilledQuantity += unitsTransacted;
            if (order.IsSelling)
            {
                order.TotalMoneyRecieved -= moneyExchanged;
                order.UnclaimedMoneyRecieved -= moneyExchanged;
            }
            else
            {
                order.TotalItemsRecieved -= unitsTransacted;
                order.UnclaimedItemsRecieved -= unitsTransacted;
            }
        }

        static async Task<StaleOrder> GetOrder(long itemId, bool isSelling, long unitValue, ComparisonPredicate unitValueComparison)
        {
            Expression<Func<StaleOrder, bool>> predicate;
            switch (unitValueComparison)
            {
                case ComparisonPredicate.GreaterThan:
                    predicate = o
                        => o.ItemId == itemId
                        && o.IsSelling == isSelling
                        && o.UnfulfilledQuantity != 0
                        && o.UnitValue >= unitValue;
                    break;
                case ComparisonPredicate.LessThan:
                    predicate = o
                        => o.ItemId == itemId
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

        static async Task InsertOrder(StaleOrder staleOrder)
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

        static async Task<VersionedUpdateResult> UpdateStaleOrder(StaleOrder staleOrder)
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
    }

    internal enum ComparisonPredicate
    {
        GreaterThan,
        LessThan
    }
}
