using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using Microsoft.AspNet.Identity;
using StackExchange.Redis;

namespace Caroline.Persistence.Models
{
    public partial class User : IUser<long>, IIdentifiableEntity<long>, IIdentifiableEntity<RedisKey>
    {
        // User identity generation?
        public long Id { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, long> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        RedisKey IIdentifiableEntity<RedisKey>.Id
        {
            get { return Id.ToStringInvariant(); }
            set { Id = long.Parse(value, CultureInfo.InvariantCulture); }
        }
    }

    public partial class UserLogin
    {
        public static implicit operator UserLoginInfo(UserLogin login)
        {
            return new UserLoginInfo(login.LoginProvider, login.ProviderKey);
        }

        public static implicit operator UserLogin(UserLoginInfo login)
        {
            return new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            };
        }
    }
}
