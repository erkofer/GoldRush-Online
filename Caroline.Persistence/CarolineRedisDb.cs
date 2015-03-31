﻿using System;
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
            ILongTable ids;
            return new CarolineRedisDb
            {
                Ids = ids = db.SetLong("idincr"),
                Games = db.Set<Game>("g"),
                Users = db.Set<User>("u", ids),
                UserLocks = db.SetLockLong<User>("u-l", TimeSpan.FromSeconds(10)),
                GameSessions = db.Set<GameSession>("c", TimeSpan.FromMinutes(2)),
                UserNames = db.SetString("uu"),
                Logins = db.SetString("ul"),
                Emails = db.SetString("ue")
            };
        }

        public IPessimisticLockTable<User> UserLocks { get; set; }

        public ILongTable Ids { get; set; }

        public IEntityTable<Game> Games { get; private set; }

        public IAutoKeyEntityTable<User> Users { get; private set; }

        public IEntityTable<GameSession> GameSessions { get; set; }

        public IStringTable UserNames { get; set; }
        public IStringTable Logins { get; set; }
        public IStringTable Emails { get; set; }
    }
}