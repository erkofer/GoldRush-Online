using System;
using System.Configuration;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using Nito.AsyncEx;
using StackExchange.Redis;

namespace Caroline.Persistence
{
    public class CarolineRedisDb
    {
        static ConnectionMultiplexer _connection;
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();
        static RedisDbMultiplexer _redisConnection;

        CarolineRedisDb() { }

        public static async Task<CarolineRedisDb> CreateAsync()
        {
            using (await StaticInitializationLock.LockAsync())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_connection == null)
                {
                    var connectionString = ConfigurationManager.AppSettings.Get("redisConnectionString");
                    var config = ConfigurationOptions.Parse(connectionString);
                    _connection = ConnectionMultiplexer.Connect(config);
                    _redisConnection = await RedisDbMultiplexer.CreateAsync(_connection);
                }
            }

            return CreateDb();
        }

        public static CarolineRedisDb Create()
        {
            using (StaticInitializationLock.Lock())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_connection == null)
                {
                    var connectionString = ConfigurationManager.AppSettings.Get("redisConnectionString");
                    var config = ConfigurationOptions.Parse(connectionString);
                    _connection = ConnectionMultiplexer.Connect(config);
                    // If this throws an exception, run
                    // GoldRush-Online\packages\Redis-64.2.8.17\redis-server.exe
                    _redisConnection = RedisDbMultiplexer.Create(_connection);
                }
            }

            return CreateDb();
        }

        static CarolineRedisDb CreateDb()
        {
            var db = _redisConnection.Connect();
            var ret = new CarolineRedisDb
            {
                Games = db.SetLong<Game>("g"),
                Users = db.SetLong<User>("u"),
                UserIdIncrement = db.IdManager<User>("u-id"),
                UserLocks = db.LockLong("u-l", TimeSpan.FromSeconds(10)),
                GameSessions = db.Set<GameSession, GameSessionEndpoint>("s", TimeSpan.FromMinutes(2)),
                UserNames = db.String("uu"),
                Logins = db.String("ul"),
                Emails = db.String("ue")
            };
            return ret;
        }

        public IPessimisticLockTable<long> UserLocks { get; private set; }

        public IIdManager<User> UserIdIncrement { get; private set; }

        public IEntityTable<Game, long> Games { get; private set; }

        public IEntityTable<User, long> Users { get; private set; }

        public IEntityTable<GameSession, GameSessionEndpoint> GameSessions { get; private set; }

        public IStringTable UserNames { get; private set; }
        public IStringTable Logins { get; private set; }
        public IStringTable Emails { get; private set; }
    }
}
