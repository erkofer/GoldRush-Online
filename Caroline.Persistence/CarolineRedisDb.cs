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

        public static async Task<CarolineRedisDb> Create()
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
                    _redisConnection = await RedisDbMultiplexer.Create(_connection);
                }
            }

            var db = _redisConnection.Connect();
            ILongTable ids;
            return new CarolineRedisDb
            {
                Ids = ids = db.SetLong(1),
                Games = db.Set<Game>(2),
                Users = db.Set<User>(3, ids),
                GameSessions = db.Set<GameSession>(4, TimeSpan.FromSeconds(60)),
                UserNames = db.SetString(5),
                Logins = db.SetString(6),
                Emails = db.SetString(7)
            };
        }

        public ILongTable Ids { get; set; }

        public IEntityTable<Game> Games { get; private set; }

        public IAutoKeyEntityTable<User> Users { get; private set; }

        public IEntityTable<GameSession> GameSessions { get; set; }

        public IStringTable UserNames { get; set; }
        public IStringTable Logins { get; set; }
        public IStringTable Emails { get; set; }
    }
}
