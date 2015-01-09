using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Caroline.Domain;
using Caroline.Extensions;
using Caroline.Models;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Caroline.Api
{
    public static class AnonymousProfileApi
    {
        const string AnonymousProfileCookieName = "GoldRushAnonymousId";

        public static bool GenerateAnonymousProfileIfNotAuthenticated(HttpContextBase httpContext, string[] usernamesWhiteList = null, string[] rolesWhiteList = null)
        {
            RegisterAnonymouslyIfLoggedOff(httpContext);

            // assert that the user is whitelisted
            return (usernamesWhiteList == null || usernamesWhiteList.Length == 0 || usernamesWhiteList.Contains(httpContext.User.Identity.Name))
                && (rolesWhiteList == null || rolesWhiteList.Length == 0 || rolesWhiteList.Any(httpContext.User.IsInRole));
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
}