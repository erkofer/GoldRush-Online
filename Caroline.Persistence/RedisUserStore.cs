using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;
using Microsoft.AspNet.Identity;

namespace Caroline.Persistence
{
    public class UserStore : IUserLoginStore<User, long>, IUserPasswordStore<User, long>, IUserSecurityStampStore<User, long>, IUserEmailStore<User, long>
    {
        readonly IStringTable _userNameLookup;
        readonly IStringTable _emailsLookup;
        readonly IStringTable _loginsLookup;
        readonly IAutoKeyEntityTable<User> _users;
        bool _disposed;

        public UserStore(CarolineRedisDb db)
        {
            _users = db.Users;
            _userNameLookup = db.UserNames;
            _loginsLookup = db.Logins;
            _emailsLookup = db.Emails;
        }

        #region IUserLoginStore Implementation
        public Task CreateAsync(User user)
        {
            return UpdateAsync(user, SetMode.Add);
        }

        public Task UpdateAsync(User user)
        {
            return UpdateAsync(user, SetMode.Overwrite);
        }

        async Task<bool> UpdateAsync(User user, SetMode mode)
        {
            Check(user);
            await _users.Set(user, mode);
            var userId = VarintBitConverter.GetVarintBytes(user.Id).GetStringNoEncoding();
            await _userNameLookup.Set(user.UserName, userId);
            await _emailsLookup.Set(user.Email, userId);
            foreach (UserLogin login in user.Logins)
            {
                await _loginsLookup.Set(GetLoginKey(login), userId);
            }
            return true;
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
            var id = VarintBitConverter.ToInt64(userId.GetBytesNoEncoding());
            return await _users.Get(id);
        }

        public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            Check(user, login);
            var userLogin = new UserLogin { LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey };
            user.Logins.Add(userLogin);

            var dbUser = await _users.Get(user.Id);
            dbUser.Logins.Add(userLogin);
            await _users.Set(dbUser, SetMode.Overwrite);
            await _loginsLookup.Set(GetLoginKey(login), VarintBitConverter.GetVarintBytes(dbUser.Id).GetStringNoEncoding());
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
            await _users.Set(dbUser, SetMode.Overwrite);
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
            return await _users.Get(VarintBitConverter.ToInt64(userId.GetBytesNoEncoding()));
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
            return await _users.Get(VarintBitConverter.ToInt64(userId.GetBytesNoEncoding()));
        }

        #endregion

        public void Dispose()
        {
            _disposed = true;
        }

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
    }
}