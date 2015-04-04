using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis.Extensions;
using JetBrains.Annotations;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RedisPessimisticLockTable<TId> : IPessimisticLockTable<TId>
    {
        readonly IDatabase _db;
        private readonly ISerializer<TId> _keySerializer;
        readonly TimeSpan _defaultExpiry;
        readonly CarolineScriptsRepo _scripts;

        public RedisPessimisticLockTable(IDatabaseArea db, ISerializer<TId> keySerializer, TimeSpan defaultExpiry)
        {
            _db = db;
            _keySerializer = keySerializer;
            _defaultExpiry = defaultExpiry;
            _scripts = db.Scripts;
        }

        public async Task<IAsyncDisposable> Lock(TId id)
        {
            var tid = _keySerializer.Serialize(id);
            for (var i = 0; i < 15; i++) // run 1.5x times the length of _defaultExpiry
            {
                var lockSucess = await _db.TryLock(_scripts, tid, _defaultExpiry);
                if (lockSucess.IsLockAquired)
                    return new DisposableCallback(() => _db.KeyDeleteAsync(tid));

                // locking failed, wait a random backoff
                var randDouble = RandomSingleton.NextDouble();
                var delay = TimeSpan.FromSeconds(i * ( _defaultExpiry.TotalMilliseconds * randDouble * .05 + _defaultExpiry.TotalMilliseconds * .125) / 5);
                await Task.Delay(delay);
            }

            // locking failed every time
            throw new TimeoutException();
        }

        class DisposableCallback : IAsyncDisposable
        {
            Func<Task> _callback;

            public DisposableCallback([NotNull] Func<Task> callback)
            {
                if (callback == null) throw new ArgumentNullException("callback");
                _callback = callback;
            }

            public async Task DisposeAsync()
            {
                // nullify _callback so that it is only called once during dispose.
                if (_callback == null) return;
                await _callback();
                _callback = null;
            }
        }
    }

    public interface IPessimisticLockTable<in TId>
    {
        Task<IAsyncDisposable> Lock(TId id);
    }
}
