using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Caroline.Persistence
{
    public class RedisUserStore<TUser, TKey> : IUserStore<TUser, TKey>
        where TUser : class, IUser<TKey>
    {
        public Task CreateAsync(TUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(TKey userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(TUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(TUser user)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
