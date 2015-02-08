using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.RedisScripts;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisDbMultiplexer
    {
        readonly ConnectionMultiplexer _connection;
        readonly CarolineScriptsRepo _scripts;

        public static async Task<RedisDbMultiplexer> Create(ConnectionMultiplexer connection)
        {
            var luaScripts = new EmbeddedResourcesDictionary(typeof(ScriptsNamespace));
            var scripts = await CarolineScriptsRepo.Create(luaScripts, connection.GetEndPoints().Select(socket => connection.GetServer(socket)));
            return new RedisDbMultiplexer(connection, scripts);
        }

        RedisDbMultiplexer(ConnectionMultiplexer connection, CarolineScriptsRepo scripts)
        {
            _connection = connection;
            _scripts = scripts;
        }

        public RedisDb Connect(int database = 0, byte[] keyPrefix = null)
        {
            return new RedisDb(_connection.GetDatabase(database), _scripts, keyPrefix);
        }
    }
}
