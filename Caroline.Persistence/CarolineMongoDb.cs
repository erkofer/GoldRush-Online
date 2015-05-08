using System.Configuration;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using MongoDB.Driver;
using Nito.AsyncEx;

namespace Caroline.Persistence
{
    public class CarolineMongoDb
    {
        static MongoClient _connection;
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static IMongoDatabase _db;

        CarolineMongoDb() { }

        public static async Task<CarolineMongoDb> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_connection == null)
                {
                    _connection = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnectionString"));
                    _db = _connection.GetDatabase(ConfigurationManager.AppSettings.Get("mongoGoldRushDatabaseId"));
                }


                _orders = _db.GetCollection<Order>("Order", new MongoCollectionSettings { AssignIdOnInsert = true });

                var oi = Builders<Order>.IndexKeys;
                await _orders.Indexes.CreateOneAsync(Builders<Order>.IndexKeys.Combine(
                    oi.Ascending(o => o.ItemId),
                    oi.Ascending(o => o.IsSelling),
                    oi.Descending(o => o.FulfilledQuantity),
                    oi.Ascending(o => o.UnitValue)));
            }

            return new CarolineMongoDb();
        }

        public IMongoCollection<Order> Orders { get { return _orders; } }
        static IMongoCollection<Order> _orders { get; set; }
    }
}
