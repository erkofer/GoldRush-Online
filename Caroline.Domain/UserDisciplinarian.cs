using Caroline.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;

namespace Caroline.Domain
{
    public class UserDisciplinarian
    {
        private CarolineRedisDb _db;
        public UserDisciplinarian(CarolineRedisDb db)
        {
            _db = db;
        }

        public static TimeSpan ParseTimeString(string duration)
        {
            if (duration == "forever")
            {
                const int daysToEnd = 365*99;
                return new TimeSpan(daysToEnd,0,0,0);   
            }
            else if (duration.Contains("d"))
            {
                duration = duration.Split('d')[0];
                var days = int.Parse(duration);
                return new TimeSpan(days,0,0,0);
            }
            else if (duration.Contains("h"))
            {
                duration = duration.Split('h')[0];
                var hours = int.Parse(duration);
                return new TimeSpan(0,hours,0,0);
            }
            else if (duration.Contains("m"))
            {
                duration = duration.Split('m')[0];
                var minutes = int.Parse(duration);
                return new TimeSpan(0,0,minutes,0);
            }
            return new TimeSpan(0);
        }

        public async Task Ban(User user, TimeSpan duration)
        {
            var oldBans = new List<UserPunishment>();
            var longestBan = true;

            var now = DateTime.UtcNow;
            var newExpiryDate = now.Add(duration);
            // check if the user is already banned.
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Ban") continue;

                var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var oldExpiryDate = dt.Add(new TimeSpan(punishment.PunishmentExpiry));

                if (newExpiryDate.Subtract(oldExpiryDate) > new TimeSpan(0))
                {
                    oldBans.Add(punishment);
                }
                else
                {
                    longestBan = false;
                }
            }

            if (longestBan)
                user.Punishments.Add(new UserPunishment { PunishmentType = "Ban", PunishmentExpiry = newExpiryDate.Ticks });


            foreach (var ban in oldBans)
                user.Punishments.Remove(ban);


            await _db.Users.Set(user);
        }

        public async Task Unban(User user)
        {
            var bansToRemove = new List<UserPunishment>();
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Ban") continue;

                bansToRemove.Add(punishment);
            }

            foreach (var ban in bansToRemove)
                user.Punishments.Remove(ban);

            await _db.Users.Set(user);
        }

        public async Task<bool> IsBanned(User user)
        {
            var expiredBans = new List<UserPunishment>();
            var banned = false;
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Ban") continue;

                if (new DateTime(punishment.PunishmentExpiry) > DateTime.UtcNow)
                    banned = true;
                else
                    expiredBans.Add(punishment);
            }

            foreach (var ban in expiredBans)
                user.Punishments.Remove(ban);
            

            await _db.Users.Set(user);
            return banned;
        }

        public DateTime GetBanCompletionTime(User user)
        {
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Ban") continue;

                return new DateTime(punishment.PunishmentExpiry);
            }
            return new DateTime(0);
        }

        public DateTime GetMuteCompletionTime(User user)
        {
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Mute") continue;
                return new DateTime(punishment.PunishmentExpiry);
            }
            return new DateTime(0);
        }

        public async Task Mute(User user, TimeSpan duration)
        {
            var oldMutes = new List<UserPunishment>();
            var longestMute = true;

            var now = DateTime.UtcNow;
            var newExpiryDate = now.Add(duration);
            // check if the user is already banned.
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Mute") continue;

                var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var oldExpiryDate = dt.Add(new TimeSpan(punishment.PunishmentExpiry));

                if (newExpiryDate.Subtract(oldExpiryDate) > new TimeSpan(0))
                {
                    oldMutes.Add(punishment);
                }
                else
                {
                    longestMute = false;
                }
            }

            if (longestMute)
                user.Punishments.Add(new UserPunishment { PunishmentType = "Mute", PunishmentExpiry = newExpiryDate.Ticks });


            foreach (var mute in oldMutes)
                user.Punishments.Remove(mute);


            await _db.Users.Set(user);
        }

        public async Task Unmute(User user)
        {
            var mutesToRemove = new List<UserPunishment>();
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Mute") continue;

                mutesToRemove.Add(punishment);
            }

            foreach (var mute in mutesToRemove)
                user.Punishments.Remove(mute);

            await _db.Users.Set(user);
        }

        public async Task<bool> IsMuted(User user)
        {
            var expiredMutes = new List<UserPunishment>();
            var muted = false;
            foreach (var punishment in user.Punishments)
            {
                if (punishment.PunishmentType != "Mute") continue;

                if (new DateTime(punishment.PunishmentExpiry) > DateTime.UtcNow)
                    muted = true;
                else
                    expiredMutes.Add(punishment);
            }

            foreach (var mute in expiredMutes)
                user.Punishments.Remove(mute);


            await _db.Users.Set(user);
            return muted;
        }
    }
}
