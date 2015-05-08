using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nito.AsyncEx;

namespace Caroline.Persistence
{
    public class CarolineMongoDb
    {
        static MongoClient _connection;
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static IMongoDatabase _db;

        CarolineMongoDb(){}

        public static CarolineMongoDb Create()
        {
            using (StaticInitializationLock.Lock())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_connection == null)
                {
                    _connection = new MongoClient(ConfigurationManager.AppSettings.Get("mongoConnectionString"));
                    _db = _connection.GetDatabase(ConfigurationManager.AppSettings.Get("mongoGoldRushDatabaseId"));
                }
            }

            return new CarolineMongoDb();
        }

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
            }

            return new CarolineMongoDb();
        }

        //public IMongoCollection<MongoGame> GameSnapshots { get; set; }
    }
}
