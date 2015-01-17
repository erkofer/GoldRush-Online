using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Caroline.Domain;
using Caroline.Extensions;
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
            var cookie = context.Request[AnonymousProfileCookieName];
            AnonymousUserCookie user = null;
            if (cookie != null)
                user = JsonConvert.DeserializeObject<AnonymousUserCookie>(MachineKey.Unprotect(cookie.GetBytes()).GetString());

            var userManager = context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            if (user == null)
                return false;

            var signInResult = userManager.PasswordSignInAsync(user.UserName, user.Password, true, false);

            return signInResult.Result == SignInStatus.Success;
        }

        static void GenerateAnonymousProfile(HttpContextBase context)
        {
            while (true) // run until a unique username is created
            {
                var anonymousCookie = new AnonymousUserCookie
                {
                    UserName = Regex.Replace(GenerateBase64String(16), @"[^A-Za-z0-9]+", ""),
                    Password = GenerateBase64String(16)
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