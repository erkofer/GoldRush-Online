using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using MongoDB.Driver;
using Nito.AsyncEx;

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

        async Task InsertStaleOrder(Order order)
        {
            if (order.Version != 0)
            {
                Log.Warn("Inserting a new order with a version != 0");
                order.Version = 0;
            }
            if (order.Id != 0)
            {
                Log.Warn("Inserting a new order with an id != 0");
                order.Id = 0;
            }

            var result = await _mongo.Orders.UpdateOneAsync(o => true, new ObjectUpdateDefinition<Order>(order),
                new UpdateOptions { IsUpsert = true });
            order.Id = result.UpsertedId.AsInt64;
        }

        async Task<Order> GetOrder(long itemId, bool isSelling, long unitValue, ComparisonPredicate unitValueComparison)
        {
            Expression<Func<Order, bool>> predicate;
            switch (unitValueComparison)
            {
                case ComparisonPredicate.GreaterThan:
                    predicate = o => o.ItemId == itemId
                        && o.IsSelling == isSelling
                        && o.FulfilledQuantity != 0
                        && o.UnitValue >= unitValue;
                    break;
                case ComparisonPredicate.LessThan:
                    predicate = o => o.ItemId == itemId
                        && o.IsSelling == isSelling
                        && o.FulfilledQuantity != 0
                        && o.UnitValue <= unitValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unitValueComparison");
            }

            var result = await _mongo.Orders.FindAsync(predicate, new FindOptions<Order, Order> { Limit = 1 });
            var list = await result.ToListAsync();
            return list.SingleOrDefault();
        }

        async Task<VersionedUpdateResult> UpdateOrder(Order order)
        {
            var oldVersion = order.Version++;
            var result = await _mongo.Orders.UpdateOneAsync(o => o.Id == order.Id && o.Version == oldVersion, new ObjectUpdateDefinition<Order>(order));
            if (result.ModifiedCount == 1)
                return VersionedUpdateResult.Success;

            var findResult = await _mongo.Orders.FindAsync(o => o.Id == order.Id);
            var list = await findResult.ToListAsync();
            if (list.Count == 1)
                return VersionedUpdateResult.WrongVersion;
            if (list.Count == 0)
                return VersionedUpdateResult.DoesNotExist;
            Log.Warn("Multiple Orders with the same Id " + order.Id);
            return VersionedUpdateResult.DoesNotExist;
        }

        async Task<bool> DeleteOrder(long id)
        {
            var result = await _mongo.Orders.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount == 1;
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
