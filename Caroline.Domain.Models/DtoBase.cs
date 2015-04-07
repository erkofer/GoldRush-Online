using System;
using System.Threading.Tasks;
using Caroline.Persistence.Redis;

namespace Caroline.Domain.Models
{
    public abstract class DtoBase : IAsyncDisposable
    {
        private readonly IAsyncDisposable _entityLock;
        private bool _disposed;

        public DtoBase()
        {
            
        }

        public DtoBase(IAsyncDisposable entityLock)
        {
            Check(entityLock);
            _entityLock = entityLock;
        }

        protected void Check()
        {
            if(_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        protected void Check<T>(T nullCheck)
            where T : class 
        {
            Check();
            if(nullCheck == null)
                throw new ArgumentNullException();
        }

        protected void Check<TId>(IIdentifiableEntity<TId> a, IIdentifiableEntity<TId> b)
               where TId : class, IEquatable<TId>
        {
            Check();
            if (a == null || b == null)
                throw new ArgumentNullException();
            if (!a.Id.Equals(b))
                throw new ArgumentException("Ids do not match");
        }
        protected void CheckId<TId>(IIdentifiableEntity<TId> a, IIdentifiableEntity<TId> b)
              where TId : struct, IEquatable<TId>
        {
            Check();
            if (!a.Id.Equals(b))
                throw new ArgumentException("Ids do not match");
        }


        protected void CheckEqual<T, TId>(TId a, T b)
            where T : class, IIdentifiableEntity<TId> 
            where TId : struct, IEquatable<TId>
        {
            Check(b);
            if(!Equals(a, b.Id))
                throw new ArgumentException("Ids do not match.");
        }

        public async Task DisposeAsync()
        {
            if (_disposed)
                return;
            await _entityLock.DisposeAsync();
            _disposed = true;
        }

        protected virtual Task DisposeImplAsync()
        {
            return Task.FromResult(0);
        }
    }
}
