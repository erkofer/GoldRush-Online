using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Caroline.Domain;
using Caroline.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Caroline.Api
{
    public static class AnonymousProfileApi
    {
        const string AnonymousProfileCookieName = "GoldRushAnonymousId";

        public static bool GenerateAnonymousProfileIfNotAuthenticated(
            HttpContextBase context,
            string[] usernamesWhiteList = null,
            string[] rolesWhiteList = null)
        {
            RegisterAnonymouslyIfLoggedOff(context);

            // assert that the user is whitelisted
            return (usernamesWhiteList == null || usernamesWhiteList.Length == 0 || usernamesWhiteList.Contains(context.User.Identity.Name))
                && (rolesWhiteList == null || rolesWhiteList.Length == 0 || rolesWhiteList.Any(context.User.IsInRole));
        }

        public static async Task<IdentityResult> TryMigrateAnonymousAccountOrRegister(HttpContextBase context, RegisterViewModel model)
        {
            using (var users = context.GetOwinContext().Get<ApplicationUserManager>())
            {
                IdentityResult result;

                if (context.User.Identity.IsAuthenticated)
                {
                    // migrate from an anonymous account
                    var id = context.User.Identity.GetUserId();
                    var anonUser = await users.FindByIdAsync(id);
                    if (anonUser == null || !anonUser.IsAnonymous)
                        return new IdentityResult();
                    anonUser.IsAnonymous = false;
                    anonUser.Email = model.Email;
                    anonUser.UserName = model.UserName;
                    result = await users.UpdateAsync(anonUser);
                    Debug.Assert(result.Succeeded);
                    result = await users.RemovePasswordAsync(id);
                    Debug.Assert(result.Succeeded);
                    result = await users.AddPasswordAsync(id, model.Password);
                    Debug.Assert(result.Succeeded);

                    return result;
                }

                // register a new account
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.UserName
                };
                result = await users.CreateAsync(user, model.Password);
                var signIn = context.GetOwinContext().Get<ApplicationSignInManager>();
                await signIn.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return result;
            }
        }

        static void RegisterAnonymouslyIfLoggedOff(HttpContextBase context)
        {
            if (IsSignedIn(context))
                return;

            // Check if the user has an anonymous account and needs to sign in again
            if (TrySignInAnonymous(context))
                return;

            GenerateAnonymousProfile(context);
        }

        static bool IsSignedIn(HttpContextBase context)
        {
            return context.User.Identity.IsAuthenticated;
        }

        static bool TrySignInAnonymous(HttpContextBase context)
        {
            try
            {
                var cookie = context.Request[AnonymousProfileCookieName];
                AnonymousUserCookie user = null;
                if (cookie != null)
                    user = JsonConvert.DeserializeObject<AnonymousUserCookie>(cookie);

                if (user == null)
                    return false;
                var userManager = context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
                var username = Convert.ToBase64String(MachineKey.Unprotect(Convert.FromBase64String(user.UserName)) ?? new byte[] { });
                var password = Convert.ToBase64String(MachineKey.Unprotect(Convert.FromBase64String(user.Password)) ?? new byte[] { });
                var signInResult = userManager.PasswordSignInAsync(username, password, true, false);

                return signInResult.Result == SignInStatus.Success;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        static void GenerateAnonymousProfile(HttpContextBase context)
        {
            while (true) // run until a unique username is created
            {
                var anonymousCookie = new AnonymousUserCookie
                {
                    UserName = Convert.ToBase64String(MachineKey.Protect(Convert.FromBase64String(GenerateBase64String(16)))),
                    Password = Convert.ToBase64String(MachineKey.Protect(Convert.FromBase64String(GenerateBase64String(16))))
                };
                var user = new ApplicationUser { UserName = anonymousCookie.UserName, IsAnonymous = true };

                var userManager = context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var result = userManager.Create(user, anonymousCookie.Password);
                if (!result.Succeeded) continue;

                var signInManager = context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
                signInManager.SignIn(user, isPersistent: true, rememberBrowser: true);

                var cookie = new HttpCookie(AnonymousProfileCookieName)
                {
                    Value = JsonConvert.SerializeObject(anonymousCookie),
                    Expires = DateTime.MaxValue // expires never
                };
                context.Response.AppendCookie(cookie);

                return;
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