using System;
using System.Numerics;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Admin
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            if (args.Length == 3 && args[1] == "del")
                await Delete(args[0], args[2]);
            else
                Console.WriteLine(@"Wow ya cunt. Use the args format ""localhost del g:*"" ya fuckn ninny.");
        }

        static async Task Delete(string connectionString, string keys)
        {
            long totalDeleted = 0;
            using (var connection = await ConnectionMultiplexer.ConnectAsync(connectionString))
            {
                var db = connection.GetDatabase();
                foreach (var endpoint in connection.GetEndPoints())
                {
                    var server = connection.GetServer(endpoint);
                    foreach (var key in server.Keys(pattern: keys))
                    {
                        await db.KeyDeleteAsync(key);
                        totalDeleted++;
                    }
                }
            }
            Console.WriteLine("Deleted " + totalDeleted + " keys.");
        }
    }
}
