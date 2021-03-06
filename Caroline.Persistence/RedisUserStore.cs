﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using Microsoft.AspNet.Identity;

namespace Caroline.Persistence
{
    public class RedisUserStore :
        IUserLoginStore<User, long>,
        IUserClaimStore<User, long>,
        IUserRoleStore<User, long>,
        IUserPasswordStore<User, long>,
        IUserSecurityStampStore<User, long>,
        IQueryableUserStore<User, long>,
        IUserEmailStore<User, long>,
        IUserPhoneNumberStore<User, long>,
        IUserTwoFactorStore<User, long>,
        IUserLockoutStore<User, long>
    {
        readonly IStringTable _userNameLookup;
        readonly IStringTable _emailsLookup;
        readonly IStringTable _loginsLookup;
        readonly IStringTable _userIdLookup;
        readonly IEntityTable<User, long> _users;
        readonly IIdManager<User> _userIds; 
        bool _disposed;

        public RedisUserStore(CarolineRedisDb db)
        {
            _users = db.Users;
            _userIds = db.UserIdIncrement;
            _userNameLookup = db.UserNames;
            _loginsLookup = db.Logins;
            _userIdLookup = db.UserIds;
            _emailsLookup = db.Emails;
        }

        #region IUserLoginStore Implementation
        public async Task CreateAsync(User user)
        {
            Check(user);
            await _userIds.SetNewId(user);
            await UpdateAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            Check(user);
            await _users.Set(user);
            var userId = user.Id.ToStringInvariant();

            await _userNameLookup.Set(user.UserName, userId);
            await _userIdLookup.Set(userId, user.UserName);
            if (!string.IsNullOrEmpty(user.Email))
                await _emailsLookup.Set(user.Email, userId);
            foreach (var login in user.Logins)
                await _loginsLookup.Set(GetLoginKey(login), userId);
        }

        public async Task DeleteAsync(User user)
        {
            Check(user);
            await _users.Delete(user.Id);
            await _userNameLookup.Delete(user.UserName);
            await _emailsLookup.Delete(user.Email);
            foreach (UserLogin login in user.Logins)
            {
                await _loginsLookup.Delete(GetLoginKey(login));
            }
        }

        public Task<User> FindByIdAsync(long userId)
        {
            ThrowIfDisposed();
            return _users.Get(userId);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            Check(userName);
            var userId = await _userNameLookup.Get(userName);
            if (userId == null)
                return null;
            var id = long.Parse(userId, CultureInfo.InvariantCulture);
            return await _users.Get(id);
        }

        public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            Check(user, login);
            var userLogin = new UserLogin { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey };
            user.Logins.Add(userLogin);

            var dbUser = await _users.Get(user.Id);
            dbUser.Logins.Add(userLogin);
            await _users.Set(dbUser);
            await _loginsLookup.Set(GetLoginKey(login), dbUser.Id.ToStringInvariant());
        }

        public async Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            Check(user, login);
            var dbUser = await _users.Get(user.Id);

            var badLogins = dbUser.Logins.Where(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey);
            var goodLogins = dbUser.Logins.Except(badLogins).ToList();
            dbUser.Logins.Clear();
            dbUser.Logins.AddRange(goodLogins);
            await _loginsLookup.Delete(GetLoginKey(login));
            await _users.Set(dbUser);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            Check(user);
            var logins = (IList<UserLoginInfo>)user.Logins.Cast<UserLoginInfo>().ToList();
            return Task.FromResult(logins);
        }

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            Check(login);
            var userId = await _loginsLookup.Get(GetLoginKey(login));
            if (userId == null)
                return null;
            return await _users.Get(long.Parse(userId, CultureInfo.InvariantCulture));
        }

        static string GetLoginKey(UserLogin login)
        {
            return login.LoginProvider + ":" + login.ProviderKey;
        }

        static string GetLoginKey(UserLoginInfo login)
        {
            return login.LoginProvider + ":" + login.ProviderKey;
        }

        #endregion

        #region IUserClaimStore Implementation
        /*public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            Check(user, login);
            var userLogin = new UserLogin { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey };
            user.Logins.Add(userLogin);

            var dbUser = await _users.Get(user.Id);
            dbUser.Logins.Add(userLogin);
            await _users.Set(dbUser);
            await _loginsLookup.Set(GetLoginKey(login), dbUser.Id.ToStringInvariant());
        }*/
        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
           /* Check(user);
            var messedClaims = new List<Claim>();
            foreach (var claim in user.Claims)
            {
                messedClaims.Add(new Claim(claim.ClaimType,claim.ClaimValue));
            }
            
            //var claims = user.Claims.Cast<Claim>().ToList();
            return Task.FromResult((IList<Claim>)messedClaims);*/
            return Task.FromResult<IList<Claim>>(new List<Claim>(0));
        }

        public async Task AddClaimAsync(User user, Claim claim)
        {
            /*Check(user);
            user.Claims.Add(new UserClaim() {ClaimType = claim.Type, ClaimValue = claim.Value});

            var dbUser = await _users.Get(user.Id);
            dbUser.Claims.Add(new UserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });
            await _users.Set(dbUser);*/
        }

        public async Task RemoveClaimAsync(User user, Claim claim)
        {
            /*Check(user);
            user.Claims.Remove(new UserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });

            var dbUser = await _users.Get(user.Id);
            dbUser.Claims.Remove(new UserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });
            await _users.Set(dbUser);*/
        }

        #endregion

        #region IUserRoleStore Implementation

        public Task AddToRoleAsync(User user, string roleName)
        {
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            return Task.FromResult(false);
        }

        #endregion

        #region IUserPasswordStore Implementation

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            Check(user); // passwordHash may be null
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            Check(user);
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            Check(user);
            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        #region IUserSecurityStampStore Implementation

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            Check(user); // stamp may be null
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(User user)
        {
            Check(user);
            return Task.FromResult(user.SecurityStamp);
        }

        #endregion

        #region IQueryableUserStore Implementation

        public IQueryable<User> Users { get { return Enumerable.Empty<User>().AsQueryable(); } }
        #endregion

        #region IUserEmailStore Implementation

        public Task SetEmailAsync(User user, string email)
        {
            Check(user); // email may be null
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user)
        {
            Check(user);
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            Check(user);
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            Check(user);
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            Check(email);
            var userId = await _emailsLookup.Get(email);
            if (userId == null)
                return null;
            return await _users.Get(long.Parse(userId, CultureInfo.InvariantCulture));
        }

        #endregion

        #region IUserPhoneNumberStore Implementation

        public Task SetPhoneNumberAsync(User user, string phoneNumber)
        {
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(User user)
        {
            return Task.FromResult<string>(null);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user)
        {
            return Task.FromResult(true);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore Implementation

        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        #endregion

        #region IUserLockoutStore Implementation

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return Task.FromResult(DateTimeOffset.UtcNow);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(0);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region Helpers

        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        void Check(object obj)
        {
            ThrowIfDisposed();
            if (obj == null)
                throw new ArgumentNullException();
        }

        void Check(object obj, object obj2)
        {
            ThrowIfDisposed();
            if (obj == null || obj2 == null)
                throw new ArgumentNullException();
        }

        #endregion

        public void Dispose()
        {
            _disposed = true;
        }
    }
}