using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Caroline.Domain.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using StackExchange.Redis;

namespace Caroline.Domain
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class UserManager : UserManager<User, long>
    {
        private readonly CarolineRedisDb _db;

        public static async Task<UserManager> CreateAsync()
        {
            return new UserManager(await CarolineRedisDb.CreateAsync());
        }

        UserManager(CarolineRedisDb db)
            : base(new RedisUserStore(db))
        {
            _db = db;
        }

        public async Task<UserDto> GetUser(long id)
        {
            var ulock = await _db.UserLocks.Lock(id);
            if (ulock == null)
                throw new TimeoutException();
            return new UserDto(ulock, _db, id);
            
        }

        public async Task<IdentityResult> SetPassword(User user, string password)
        {

            var result = await PasswordValidator.ValidateAsync(password);
            if (!result.Succeeded)
            {
                return result;
            }
            var store = Store as IUserPasswordStore<User, long>;
            var hash = PasswordHasher.HashPassword(password);
            await store.SetPasswordHashAsync(user, hash);
            await UpdateSecurityStampAsync(user.Id);
            await Store.UpdateAsync(user);
            return IdentityResult.Success;
        }

        public Task<ScoreEntry[]> GetLeaderboardEntries(long start = 0, long end = long.MaxValue)
        {
            return _db.HighScores.Range("lb", start, end, Order.Descending);
        }

        public Task SetLeaderboardEntry(long userId, long value)
        {
            return _db.HighScores.Add(new ScoreEntry {ListName = "lb", UserId = userId, Score = value});
        }

        public Task<string> GetUsername(long userid)
        {
            return _db.UserIds.Get(userid.ToStringInvariant());
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(CarolineRedisDb.Create());
            // Configure validation logic for usernames
            manager.UserValidator = new GoldRushUserValidator(manager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = false;
            //manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //manager.MaxFailedAccessAttemptsBeforeLockout = 20;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class GoldRushUserValidator : UserValidator<User, long>
    {
        public GoldRushUserValidator(UserManager<User, long> manager)
            : base(manager)
        {
        }
        const string Base64Characters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        const string RegisteredCharacters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_ ";
        static readonly HashSet<char> Base64CharactersHash = new HashSet<char>(Base64Characters);
        static readonly HashSet<char> RegisteredCharactersHash = new HashSet<char>(RegisteredCharacters);

        public override async Task<IdentityResult> ValidateAsync(User item)
        {
            var baseResult = await base.ValidateAsync(item);

            var hash = item.IsAnonymous ? Base64CharactersHash : RegisteredCharactersHash;
            List<char> illegalChars = null;
            for (int i = 0; i < item.UserName.Length; i++)
            {
                var character = item.UserName[i];
                if (hash.Contains(character)) continue;

                if (illegalChars == null)
                    illegalChars = new List<char>();
                illegalChars.Add(character);
            }

            if (illegalChars != null && illegalChars.Count > 0)
            {
                return new IdentityResult(baseResult.Errors.Concat(new[] { "Your username may only contain letters, numbers, spaces and underscores." }));
            }
            return baseResult;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<User, long>
    {
        public ApplicationSignInManager(UserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync(UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }
}
