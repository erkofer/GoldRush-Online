using Caroline.Persistence.Redis;
using Microsoft.AspNet.Identity;

namespace Caroline.Persistence.Models
{
    public partial class User : IUser<long>, IIdentifiableEntity<long>
    {
        // User identity generation?
        public long Id { get; set; }
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
