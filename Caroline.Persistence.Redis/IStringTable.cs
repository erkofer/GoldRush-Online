using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public interface IStringTable
    {
        Task<string> Get(string id);
        Task<bool> Set(string id, string value, TimeSpan? expiry = null, When when = When.Always);
        Task<string> GetSet(string id, string setValue, TimeSpan? expiry = null);
        Task<bool> Delete(string id);
    }
}