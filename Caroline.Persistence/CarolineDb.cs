using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Caroline.Persistence
{
    class CarolineDb
    {
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static CarolineRedisDb _redis;
        static CarolineMongoDb _mongo;

        CarolineDb() { }

        public static CarolineDb Create()
        {
            using (StaticInitializationLock.Lock())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_redis == null)
                    _redis = CarolineRedisDb.Create();
                if (_mongo == null)
                    _mongo = CarolineMongoDb.Create();
            }

            return new CarolineDb();
        }

        public static async Task<CarolineDb> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_redis == null)
                    _redis = await CarolineRedisDb.CreateAsync();
                if (_mongo == null)
                    _mongo = await CarolineMongoDb.CreateAsync();
            }

            return new CarolineDb();
        }
    }
}
