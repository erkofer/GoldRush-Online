using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.RedisScripts;
using Nito.AsyncEx;
using StackExchange.Redis;

namespace Caroline.Persistence
{
    public class RedisDb
    {
        readonly byte[] _keyPrefix;
        readonly IDatabase _session;
        readonly HashSet<long> _claimedIds = new HashSet<long>();
        readonly RedisLongTable _ids;

        static ConnectionMultiplexer _connection;
        static ConfigurationOptions _config;
        static CarolineScriptsRepo _scripts;
        static readonly AsyncLock StaticInitializationLock = new AsyncLock();

        public static async Task<RedisDb> Create(byte[] keyPrefix = null)
        {
            using (await StaticInitializationLock.LockAsync())
            {
                // dont instantiate the multiplexer in a static constructor because if
                // it throws an exception in the static ctor, then this class becomes unusable in the AppDomain
                if (_connection == null)
                {
                    var connectionString = ConfigurationManager.AppSettings.Get("redisConnectionString");
                    _config = ConfigurationOptions.Parse(connectionString);
                    _connection = ConnectionMultiplexer.Connect(_config);

                    // eagerly load scripts
                    var luaScripts = new EmbeddedResourcesDictionary(typeof(ScriptsNamespace));
                    _scripts = await CarolineScriptsRepo.Create(luaScripts,
                        _config.EndPoints.Select(socket => _connection.GetServer(socket)));
                }
            }

            return new RedisDb(keyPrefix);
        }

        RedisDb(byte[] keyPrefix)
        {
            _session = _connection.GetDatabase();
            _keyPrefix = keyPrefix;

            _ids = new RedisLongTable(_session, 0, _keyPrefix);
            
        }

        public IEntityTable<TEntity> Set<TEntity>(long id, ISerializer<TEntity> serializer, IIdentifier<TEntity> identifier)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("id", "Id must be greater than 0.");
            if (!_claimedIds.Add(id))
                throw new ArgumentException("A set with the same typeId has already been set");

            return new RedisEntityTable<TEntity>(_session, serializer, identifier, id, () => _ids.IncrementAsync(id), _scripts, _keyPrefix);
        }
    }
}
