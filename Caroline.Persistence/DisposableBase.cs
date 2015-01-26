using System;

namespace Caroline.Persistence
{
    public abstract class DisposableBase : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            DisposeObj();

            _disposed = true;
        }

        protected abstract void DisposeObj();


        public void GuardDispose()
        {
            if(_disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}
