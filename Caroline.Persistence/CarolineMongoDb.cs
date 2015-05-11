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

        public static CarolineMongoDb Create()
        {
            using (StaticInitializationLock.Lock())
            {
                Init();
                // dont init indexes. they will most likely already be initialized, and need async calls that we cant await
            }
            return new CarolineMongoDb();
        }

        public static async Task<CarolineMongoDb> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
            {
                Init();

                var oi = Builders<StaleOrder>.IndexKeys;
                await _orders.Indexes.CreateOneAsync(oi.Combine(
                    oi.Ascending(o => o.ItemId),
                    oi.Ascending(o => o.IsSelling),
                    oi.Descending(o => o.UnfulfilledQuantity),
                    oi.Ascending(o => o.UnitValue),
                    oi.Ascending(o => o.Version)));
                await _orders.Indexes.CreateOneAsync(oi.Combine(
                    oi.Ascending(o => o.GameId),
                    oi.Ascending(o => o.Id)));
            }

            return new CarolineMongoDb();
        }

        static void Init()
        {
            // dont instantiate the multiplexer in a static constructor because if
            // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
            if (_connection == null)
                _connection = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnectionString"));

            if (_db == null)
                _db = _connection.GetDatabase(ConfigurationManager.AppSettings.Get("mongoGoldRushDatabaseId"));

            if (_orders == null)
                _orders = _db.GetCollection<StaleOrder>("Order", new MongoCollectionSettings { AssignIdOnInsert = true });
        }

        static IMongoCollection<StaleOrder> _orders;
        public IMongoCollection<StaleOrder> Orders { get { return _orders; } }
    }
}
