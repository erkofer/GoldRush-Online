using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caroline.Domain.Helpers;
using Caroline.Domain.Models;
using Caroline.Persistence;
using Caroline.Persistence.Extensions;
using Caroline.Persistence.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Nito.AsyncEx;

namespace Caroline.Domain
{
    public class MarketPlace
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

        public async Task<StaleOrder> Transact(FreshOrder order)
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
                staleOrder.UnclaimedMoneyRecieved += differenceToRefund;

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

        public Task<StaleOrder> GetOrder(string id)
        {
            ObjectId objid;
            if (ObjectIdHelpers.ParseAndLog(id, out objid))
                return _mongo.Orders.SingleOrDefault(o => o.Id == objid);
            return null;
        }

        public Task<IAsyncCursor<StaleOrder>> GetOrdersByGame(long gameId)
        {
            return _mongo.Orders.FindAsync(o => o.GameId == gameId);
        }

        public async Task<long?> ClaimOrderContents(string id, ClaimField field)
        {
            ObjectId objId;
            if (!ObjectIdHelpers.ParseAndLog(id, out objId))
                return null;
            int i;
            for (i = 0; i < 10; i++)
            {
                var order = await _mongo.Orders.SingleOrDefault(o => o.Id == objId);
                if (order == null)
                    return null;
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
                        return numClaimed;
                    case VersionedUpdateResult.WrongVersion:
                        continue; // retry
                    case VersionedUpdateResult.DoesNotExist:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Log.Warn("MarketPlace.ClaimOrderContents retried " + i + "times and failed with WrongVersion.");
            return null;
        }

        public async Task<bool> CancelOrder(string id)
        {
            ObjectId objId;
            if (!ObjectIdHelpers.ParseAndLog(id, out objId))
                return false;

            UpdateResult result;
            byte i = 0;
            do
            {
                if (i++ == 20)
                    throw new TimeoutException();

                var order = await _mongo.Orders.SingleOrDefault(o => o.Id == objId);
                if (order == null)
                    return false;

                order.UnfulfilledQuantity = 0;
                var remaining = order.UnfulfilledQuantity;
                if (order.IsSelling)
                {
                    order.TotalItemsRecieved += remaining;
                    order.UnclaimedItemsRecieved += remaining;
                }
                else
                {
                    var refund = remaining * order.UnitValue;
                    order.TotalMoneyRecieved += refund;
                    order.UnclaimedMoneyRecieved += refund;
                }

                var oldVersion = order.Version++;
                result = await _mongo.Orders.UpdateOneAsync(
                    o => o.Id == objId && o.Version == oldVersion,
                    new ObjectUpdateDefinition<StaleOrder>(order));

            } while (!result.IsModifiedCountAvailable || result.ModifiedCount != 1);
            return true;
        }

        static StaleOrder BuildStaleOrder(FreshOrder freshOrder)
        {
            return new StaleOrder
            {
                ItemId = freshOrder.ItemId,
                GameId = freshOrder.GameId,
                UnitValue = freshOrder.UnitValue,
                Quantity = freshOrder.Quantity,
                UnfulfilledQuantity = freshOrder.Quantity,
                IsSelling = freshOrder.IsSelling,
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
            if (staleOrder.Id != ObjectId.Empty)
            {
                Log.Warn("Inserting a new order with an id != 0");
                staleOrder.Id = ObjectId.Empty;
            }

            await _mongo.Orders.InsertOneAsync(staleOrder);
        }

        static async Task<VersionedUpdateResult> UpdateStaleOrder(StaleOrder staleOrder)
        {
            var oldVersion = staleOrder.Version++;
            var result = await _mongo.Orders.ReplaceOneAsync(o => o.Id == staleOrder.Id && o.Version == oldVersion, staleOrder, new UpdateOptions { IsUpsert = true });
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
