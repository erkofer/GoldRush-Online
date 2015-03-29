using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public interface ILongTable
    {
        void IncrementFaf(RedisKey key, long incrementValue = 1, TimeSpan? expiry = null);
        Task<long> IncrementAsync(RedisKey key, long incrementValue = 1, TimeSpan? expiry = null);
    }
}