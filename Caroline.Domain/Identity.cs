using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Caroline.Domain
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<GoldRushDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true
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
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 20;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public static async Task<bool> TryMigrateAnonymousAccountOrRegister(HttpContextBase context, RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
                return false;

            using (var work = new UnitOfWork())
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    // migrate from an anonymous account
                    var id = context.User.Identity.GetUserId();
                    var anonUser = await work.Users.Get(id);
                    if (!anonUser.IsAnonymous)
                        return false;

                    anonUser.IsAnonymous = false;
                    anonUser.PasswordHash = new PasswordHasher().HashPassword(model.Password);
                    anonUser.Email = model.Email;
                    anonUser.UserName = anonUser.UserName;
                    work.Users.Update(anonUser);
                    await work.SaveChangesAsync();

                    return true;
                }

                // register a new account
                var store = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
                var user = new ApplicationUser();
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await store.CreateAsync(user, model.Password);
                return result.Succeeded;
            }
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
