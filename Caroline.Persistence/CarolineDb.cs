using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Caroline.Persistence
{
    public class CarolineDb
    {
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static CarolineMongoDb _mongo;

        CarolineDb(CarolineRedisDb redis)
        {
            Redis = redis;
        }

        public static CarolineDb Create()
        {
            using (StaticInitializationLock.Lock())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_mongo == null)
                    _mongo = CarolineMongoDb.Create();
            }

            return new CarolineDb(CarolineRedisDb.Create());
        }

        public static async Task<CarolineDb> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_mongo == null)
                    _mongo = await CarolineMongoDb.CreateAsync();
            }

            return new CarolineDb(await CarolineRedisDb.CreateAsync());
        }

        public CarolineRedisDb Redis { get; private set; }
        public CarolineMongoDb Mongo { get { return _mongo; } } // single mongo instance
    }
}
