using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Caroline.Domain;
using Caroline.Models;
using Caroline.Persistence.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Caroline.Api
{
    public static class AnonymousProfileApi
    {
        const string AnonymousProfileCookieName = "GoldRushAnonymousId";

        public static Task GenerateAnonymousProfileIfNotAuthenticated(HttpContextBase context)
        {
            return RegisterAnonymouslyIfLoggedOff(context);
        }

        public static async Task<bool> GenerateAnonymousProfileIfNotAuthenticated(
            HttpContextBase context,
            string[] usernamesWhiteList,
            string[] rolesWhiteList = null)
        {
            await GenerateAnonymousProfileIfNotAuthenticated(context);

            // assert that the user is whitelisted
            return (usernamesWhiteList == null || usernamesWhiteList.Length == 0 || usernamesWhiteList.Contains(context.User.Identity.Name))
                && (rolesWhiteList == null || rolesWhiteList.Length == 0 || rolesWhiteList.Any(context.User.IsInRole));
        }

        public static async Task<IdentityResult> TryMigrateAnonymousAccountOrRegister(HttpContextBase context, RegisterViewModel model)
        {
            using (var users = context.GetOwinContext().Get<UserManager>())
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    // migrate from an anonymous account
                    var id = context.User.Identity.GetUserId<long>();

                    var passwordResult = await users.PasswordValidator.ValidateAsync(model.Password);
                    if (!passwordResult.Succeeded)
                        return passwordResult;

                    var anonUser = await users.FindByIdAsync(id);
                    if (!anonUser.IsAnonymous)
                        return new IdentityResult("User is already registered.");
                    anonUser.IsAnonymous = false;
                    anonUser.Email = model.Email;
                    anonUser.UserName = model.UserName;
                    var result = await users.UpdateAsync(anonUser);

                    if (!result.Succeeded)
                        return result;
                    return await users.SetPassword(anonUser, model.Password);
                }

                // register a new account
                var user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName
                };
                var createResult = await users.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                    return createResult;
                var signIn = context.GetOwinContext().Get<ApplicationSignInManager>();
                await signIn.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return createResult;
            }
        }

        static async Task RegisterAnonymouslyIfLoggedOff(HttpContextBase context)
        {
            if (IsSignedIn(context))
                return;

            // Check if the user has an anonymous account and needs to sign in again
            if (await TrySignInAnonymous(context))
                return;

            await GenerateAnonymousProfile(context);
        }

        static bool IsSignedIn(HttpContextBase context)
        {
            return context.User.Identity.IsAuthenticated;
        }

        static async Task<bool> TrySignInAnonymous(HttpContextBase context)
        {
            var cookie = context.Request[AnonymousProfileCookieName];
            AnonymousUserCookie user = null;
            if (cookie != null)
                user = JsonConvert.DeserializeObject<AnonymousUserCookie>(cookie);

            if (user == null)
                return false;
            var userManager = context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            var username = Unprotect(user.UserName);
            var password = Unprotect(user.Password);
            if (username == null || password == null)
                return false;

            var signInResult = await userManager.PasswordSignInAsync(username, password, true, false);
            return signInResult == SignInStatus.Success;
        }

        static async Task GenerateAnonymousProfile(HttpContextBase context)
        {
            for (int i = 0; i < 10; i++) // run until a unique username is created
            {
                var anonymousCookie = new AnonymousUserCookie
                {
                    UserName = Protect(GenerateBase64String(8)),
                    Password = Protect(GenerateBase64String(8))
                };
                var user = new User { UserName = anonymousCookie.UserName, IsAnonymous = true };

                var userManager = context.GetOwinContext().GetUserManager<UserManager>();
                var result = userManager.Create(user, anonymousCookie.Password);
                if (!result.Succeeded) continue;

                var signInManager = context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
                await signInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);

                var cookie = new HttpCookie(AnonymousProfileCookieName)
                {
                    Value = JsonConvert.SerializeObject(anonymousCookie),
                    Expires = DateTime.MaxValue // expires never
                };
                context.Response.AppendCookie(cookie);

                return;
            }
        }

        [CanBeNull]
        static string Unprotect(string encryptedBase64String)
        {
            if (encryptedBase64String == null)
                throw new ArgumentNullException();

            try
            {
                var a = Convert.FromBase64String(encryptedBase64String);
                var b = MachineKey.Unprotect(a) ?? new byte[0];
                return Convert.ToBase64String(b);
            }
            catch
            {
                return null;
            }
        }

        [CanBeNull]
        static string Protect(string value)
        {
            try
            {
                var a = Convert.FromBase64String(value);
                var b = MachineKey.Protect(a);
                return Convert.ToBase64String(b);
            }
            catch
            {
                return null;
            }
        }

        static string GenerateBase64String(int numBytes)
        {
            var randomBytes = new byte[numBytes];
            using (var crypto = new RNGCryptoServiceProvider())
                crypto.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    //public enum AnonymousAuthentication : byte
    //{
    //    /// <summary>
    //    /// A anonymous profile will be generated if the user is not signed in.
    //    /// </summary>
    //    Generate,
    //    /// <summary>
    //    /// The user can't be registered. An anonymous profile will be created if they are not signed in.
    //    /// </summary>
    //    Require,
    //    /// <summary>
    //    /// Only registered users are allowed.
    //    /// </summary>
    //    Forbid
    //}
}