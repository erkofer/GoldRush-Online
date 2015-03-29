using System;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IStringTable
    {
        Task<string> Get(string id);
        Task<bool> Set(string id, string value, TimeSpan? expiry = null);
        Task<string> GetSet(string id, string setValue, TimeSpan? expiry = null);
        Task<bool> Delete(string id);
    }
}