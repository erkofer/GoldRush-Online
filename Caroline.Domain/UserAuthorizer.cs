using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;

namespace Caroline.Domain
{
    public class UserAuthorizer
    {
        private CarolineRedisDb _db;
        public UserAuthorizer(CarolineRedisDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds a claim to the specified user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task AddClaim(User user, UserClaim claim)
        {
            // if the user already has this claim we will not readd it.
            foreach (var uc in user.Claims)
                if (uc.ClaimValue == claim.ClaimValue) return;

            user.Claims.Add(claim);
            await _db.Users.Set(user);
        }

        /// <summary>
        /// Removes a claim from the specified user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task RemoveClaim(User user, UserClaim claim)
        {
            var claimsToRemove = new List<UserClaim>();
            foreach (var uc in user.Claims)
                if (uc.ClaimValue == claim.ClaimValue) claimsToRemove.Add(uc);

            foreach (var uc in claimsToRemove)
                user.Claims.Remove(uc);

            await _db.Users.Set(user);
        }


        public static bool IsAdministrator(User user)
        {
            foreach (var claim in user.Claims)
                if (claim.ClaimValue == "Administrator") return true;

            return false;
        }

        public static bool IsModerator(User user)
        {
            foreach (var claim in user.Claims)
                if (claim.ClaimValue == "Moderator") return true;

            return false;
        }

        public static bool IsAlphaVeteran(User user)
        {
            foreach (var claim in user.Claims)
                if (claim.ClaimValue == "Alpha") return true;

            return false;
        }
    }
}
