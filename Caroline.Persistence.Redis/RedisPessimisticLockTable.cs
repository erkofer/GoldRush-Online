using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisPessimisticLockTable<TEntity> : RedisEntityTableBase<TEntity, RedisKey>, IPessimisticLockTable<TEntity>
    {
        readonly IDatabase _db;
        readonly TimeSpan _defaultExpiry;
        readonly CarolineScriptsRepo _scripts;

        public RedisPessimisticLockTable(IDatabaseArea db, ISerializer<TEntity> serializer, IIdentifier<TEntity, RedisKey> identifier, TimeSpan defaultExpiry)
            : base(serializer, identifier)
        {
            _db = db;
            _defaultExpiry = defaultExpiry;
            _scripts = db.Scripts;
        }

        public async Task<IDisposable> Lock(TEntity id)
        {
            RedisKey tid = Identifier.GetId(id);
            for (int i = 0; i < 15; i++) // run 1.5x times the length of _defaultExpiry
            {
                var lockSucess = await _db.TryLock(_scripts, tid, _defaultExpiry);
                if (lockSucess.IsLockAquired)
                    return new DisposableCallback(async () => await _db.KeyDeleteAsync(tid));

                // locking failed, wait a random backoff
                var delay = TimeSpan.FromSeconds(_defaultExpiry.Ticks * RandomSingleton.NextDouble() * .05 + _defaultExpiry.Ticks * .125);
                await Task.Delay(delay);
            }

            // locking failed every time
            throw new TimeoutException();
        }

        class DisposableCallback : IDisposable
        {
            Action _callback;

            public DisposableCallback([NotNull] Action callback)
            {
                if (callback == null) throw new ArgumentNullException("callback");
                _callback = callback;
            }

            public void Dispose()
            {
                // nullify _callback so that it is only called once during dispose.
                if (_callback == null) return;
                _callback();
                _callback = null;
            }
        }
    }

    public interface IPessimisticLockTable<in TEntity>
    {
        Task<IDisposable> Lock(TEntity id);
    }
}
