using System;

namespace Caroline.Persistence.Redis.RedisScripts
{
    public struct TryLockResult
    {
        public bool IsLockAquired { get; private set; }
        public TimeSpan TimeToExpire { get; private set; }

        public TryLockResult(bool isLockAquired, TimeSpan timeToExpire)
            : this()
        {
            IsLockAquired = isLockAquired;
            TimeToExpire = timeToExpire;
        }
    }
}
