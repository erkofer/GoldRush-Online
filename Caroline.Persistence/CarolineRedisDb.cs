using System.Threading.Tasks;
using Caroline.Persistence.Extensions;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public class CarolineRedisDb
    {
        public static async Task<CarolineRedisDb> Create(byte[] keyPrefix)
        {
            var db = await RedisDb.Create(keyPrefix);
            return new CarolineRedisDb
            {
                Games = db.Set<Game>(1),
                Users = db.Set<Game>(2),
            };
        }

        public IEntityTable<Game> Users { get; private set; }

        public IEntityTable<Game> Games { get; private set; }
    }
}
