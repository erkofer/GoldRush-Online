using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
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
            var db = await CarolineRedisDb.CreateAsync();
            return new UserManager(db, new RedisUserStore(db));
        }

        UserManager(CarolineRedisDb db, IUserStore<User, long> store)
            : base(store)
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
            var db = CarolineRedisDb.Create();
            var store = new RedisUserStore(db);
            var manager = new UserManager(db, store);
            // Configure validation logic for usernames
            manager.UserValidator = new GoldRushUserValidator(manager, store)
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
        readonly UserManager<User, long> _manager;
        private readonly IUserEmailStore<User, long> _emailStore;

        public GoldRushUserValidator(UserManager<User, long> manager, IUserEmailStore<User,long> emailStore)
            : base(manager)
        {
            _manager = manager;
            _emailStore = emailStore;
        }

        const string Base64Characters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        const string RegisteredCharacters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_ ";
        static readonly HashSet<char> Base64CharactersHash = new HashSet<char>(Base64Characters);
        static readonly HashSet<char> RegisteredCharactersHash = new HashSet<char>(RegisteredCharacters);

        public override async Task<IdentityResult> ValidateAsync(User item)
        {
            var baseResult = await base.ValidateAsync(item);
            var errors = baseResult.Errors.ToList();

            // require unique email if the user is not anonymous
            if (!item.IsAnonymous)
                await ValidateEmail(item, errors);

            var hash = item.IsAnonymous ? Base64CharactersHash : RegisteredCharactersHash;
            List<char> illegalChars = null;
            for (var i = 0; i < item.UserName.Length; i++)
            {
                var character = item.UserName[i];
                if (hash.Contains(character)) continue;

                if (illegalChars == null)
                    illegalChars = new List<char>();
                illegalChars.Add(character);
            }

            if (illegalChars != null && illegalChars.Count > 0)
                errors.Add("Your username may only contain letters, numbers, spaces and underscores.");
            
            return errors.Count > 0 ? new IdentityResult(errors) : IdentityResult.Success;
        }

        // make sure email is not empty, valid, and unique
        // pulled from Microsoft.AspNet.Identity.UserValidator.ValidateEmail
        private async Task ValidateEmail(User user, List<string> errors)
        {
            var email = await _emailStore.GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("The Email field is required.");
                return;
            }
            try
            {
                var m = new MailAddress(email);
            }
            catch (FormatException)
            {
                errors.Add("The Email field is not a valid email address.");
                return;
            }
            var owner = await _manager.FindByEmailAsync(email);
            if (owner != null && owner.Id != user.Id)
            {
                errors.Add(String.Format("Email {0} is already taken.", user.Email));
            }
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
