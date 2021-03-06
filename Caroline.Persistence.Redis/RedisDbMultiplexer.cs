﻿using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.RedisScripts;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisDbMultiplexer
    {
        readonly ConnectionMultiplexer _connection;
        readonly CarolineScriptsRepo _scripts;

        public static async Task<RedisDbMultiplexer> CreateAsync(ConnectionMultiplexer connection)
        {
            var luaScripts = new EmbeddedResourcesDictionary(typeof(ScriptsNamespace));
            var scripts = await CarolineScriptsRepo.CreateAsync(luaScripts, connection.GetEndPoints().Select(socket => connection.GetServer(socket)));
            return new RedisDbMultiplexer(connection, scripts);}

        public static RedisDbMultiplexer Create(ConnectionMultiplexer connection)
        {
            var luaScripts = new EmbeddedResourcesDictionary(typeof(ScriptsNamespace));
            var scripts = CarolineScriptsRepo.Create(luaScripts, connection.GetEndPoints().Select(socket => connection.GetServer(socket)));
            return new RedisDbMultiplexer(connection, scripts);
        }

        RedisDbMultiplexer(ConnectionMultiplexer connection, CarolineScriptsRepo scripts)
        {
            _connection = connection;
            _scripts = scripts;
        }
        
        public IDatabaseArea Connect(int database = 0)
        {
            return new RootDatabaseArea(_connection.GetDatabase(database), _scripts);
        }
    }
}
