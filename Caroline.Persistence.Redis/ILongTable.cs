using System;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface ILongTable
    {
        void IncrementFaf(long id, long incrementValue = 1, TimeSpan? expiry = null);
        Task<long> IncrementAsync(long id, long incrementValue = 1, TimeSpan? expiry = null);
    }
}