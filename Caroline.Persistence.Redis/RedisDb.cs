using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisDb
    {
        readonly byte[] _keyPrefix;
        readonly IDatabase _session;
        readonly CarolineScriptsRepo _scripts;
        readonly HashSet<long> _claimedIds = new HashSet<long>();
        readonly RedisLongTable _ids;
        
        internal RedisDb(IDatabase db, CarolineScriptsRepo scripts, byte[] keyPrefix)
        {
            _session = db;
            _scripts = scripts;
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
