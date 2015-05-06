using System;
using System.Configuration;
using System.Globalization;
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
                Games = db.SetLong<SaveState>("g"),
                Users = db.SetLong<User>("u"),
                UserIdIncrement = db.IdManager<User>("u-id"),
                UserLocks = db.LockLong("u-l", TimeSpan.FromSeconds(10)),
                GameSessions = db.Set<GameSession, GameSessionEndpoint>("s", TimeSpan.FromMinutes(2)),
                UserNames = db.String("uu"),
                UserIds = db.String("ui"),
                Logins = db.String("ul"),
                Emails = db.String("ue"),
                HighScores = db.SetSortedList("lb",
                    ScoreIdSerializer,
                    DatabaseAreaEx.Objects.StringSerializer,
                    DatabaseAreaEx.Objects<ScoreEntry, string>.Identifier, DatabaseAreaEx.Objects<ScoreEntry, double>.Identifier),

                ChatroomMessages = db.SetSortedListString<ChatroomMessage>("cm"),
                ChatroomMessagesIdIncrement = db.Long("cm-lng"),
                ChatroomSubscribers = db.SetHashStringLong<ChatroomSubscriber>("cs"),
                ChatroomInvitations = db.SetHashStringLong<ChatroomInvitation>("ci"),
                ChatroomOptions = db.SetString<ChatroomOptions>("co"),
                UserChatroomSubscriptions = db.SetHashLongString<ChatroomSubscription>("ucs"),
                UserChatroomNotifications = db.SetListLong<ChatroomNotification>("ucn")
            };
            return ret;
        }

        public ISortedSetTable<ChatroomMessage, string> ChatroomMessages { get; private set; }
        public RedisLongTable ChatroomMessagesIdIncrement { get; private set; }

        public IEntityHashTable<ChatroomSubscriber, string, long> ChatroomSubscribers { get; private set; }

        public IEntityHashTable<ChatroomInvitation, string, long> ChatroomInvitations { get; private set; }

        public IEntityTable<ChatroomOptions, string> ChatroomOptions { get; private set; }

        public IEntityHashTable<ChatroomSubscription, long, string> UserChatroomSubscriptions { get; private set; }

        public IEntityListTable<ChatroomNotification, long> UserChatroomNotifications { get; private set; }

        public ISortedSetTable<ScoreEntry, string> HighScores { get; private set; }

        public IPessimisticLockTable<long> UserLocks { get; private set; }

        public IIdManager<User> UserIdIncrement { get; private set; }

        public IEntityTable<SaveState, long> Games { get; private set; }

        public IEntityTable<User, long> Users { get; private set; }

        public IEntityTable<GameSession, GameSessionEndpoint> GameSessions { get; private set; }

        public IStringTable UserNames { get; private set; }
        public IStringTable UserIds { get; private set; }
        public IStringTable Logins { get; private set; }
        public IStringTable Emails { get; private set; }

        static readonly ScoreUserIdSerializer ScoreIdSerializer = new ScoreUserIdSerializer();
        class ScoreUserIdSerializer : ISerializer<ScoreEntry>
        {
            public byte[] Serialize(ScoreEntry entity)
            {
                return (RedisKey)entity.UserId.ToStringInvariant();
            }

            public ScoreEntry Deserialize(byte[] data)
            {
                return new ScoreEntry { UserId = long.Parse((RedisKey)data, CultureInfo.InvariantCulture) };
            }
        }
    }
}
