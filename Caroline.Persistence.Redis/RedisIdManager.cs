using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisIdManager<TEntity> : IIdManager<TEntity>
    {
        readonly IDatabase _db;
        private readonly IIdentifier<TEntity, long> _identifier;

        public RedisIdManager(IDatabaseArea db, IIdentifier<TEntity, long> identifier)
        {
            _db = db;
            _identifier = identifier;
        }

        public async Task SetNewId(TEntity entity)
        {
            _identifier.SetId(entity, await _db.StringIncrementAsync(""));
        }
    }
}
