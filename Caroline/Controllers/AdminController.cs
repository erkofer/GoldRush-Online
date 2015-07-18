using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Caroline.App;
using Caroline.Domain;
using Caroline.Domain.Models;
using Caroline.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;

namespace Caroline.Controllers
{
    public class AdminController : Controller
    {
        public async Task<ActionResult> Index()
        {
            if (await IsAdministrator())
            {
                return View();
            }
            else
            {
                return View("AccessDenied");
            }
        }

        public async Task<ActionResult> Account(string id, string command)
        {
            if (!await IsAdministrator()) return View("AccessDenied");

            if (id == null)
            {
                ViewBag.Message = "No user specified.";
                return View("Error");
            }

            var user = await GetUser(id);
            var db = await CarolineRedisDb.CreateAsync();

            if (user == null)
            {
                ViewBag.Message = "User does not exist. Username is case sensitive.";
                return View("Error");
            }
            switch (command)
            {
                case null:
                    {
                        return await DisplayUser(user, db);
                    }
                case "add-claim":
                    {
                        return await AddClaim(db, user);
                    }
                case "remove-claim":
                    {
                        return await RemoveClaim(db, user);
                    }
                case "ban":
                    {
                        return await Ban(db, user);
                    }
                case "unban":
                    {
                        return await Unban(db, user);
                    }
                case "mute":
                    {
                        return await Mute(db, user);
                    }
                case "unmute":
                    {
                        return await Unmute(db, user);
                    }
            }
            return View("Error");
        }

        public async Task<ActionResult> Notifications(string id, string command)
        {
            if (!await IsAdministrator()) return View("AccessDenied");
            var notificationManager = await NotificationManager.CreateAsync();
            switch (id)
            {
                case null:
                {
                    return await DisplayNotifications(notificationManager);
                }
                case "add":
                {
                    var message = Request.QueryString["message"];
                    var type = Request.QueryString["type"];
                    await notificationManager.PushNotification(message, type);

                    return await DisplayNotifications(notificationManager);
                }
                case "remove":
                {
                    var message = Request.QueryString["message"];
                    var type = Request.QueryString["type"];
                    await notificationManager.RemoveNotification(message, type);

                    return await DisplayNotifications(notificationManager);
                }
            }
            return View("Error");
        }

        private async Task<ActionResult> DisplayNotifications(NotificationManager notificationManager)
        {
            var notificationsModel = new NotificationsViewModel();
            var notifications = await notificationManager.GetNotifications();

            foreach (var notification in notifications)
                notificationsModel.Notifications.Add(new Notification()
                {
                    Message = notification.Message,
                    Type = notification.Type
                });

            return View(notificationsModel);
        }

        private async Task<ActionResult> Unmute(CarolineRedisDb db, User user)
        {
            var disciplinarian = new UserDisciplinarian(db);
            await disciplinarian.Unmute(user);
            ViewBag.Message = "Unmuted " + user.UserName + ".";
            return View("Success");
        }

        private async Task<ActionResult> Mute(CarolineRedisDb db, User user)
        {
            var duration = Request.QueryString["duration"];
            if (duration == null)
            {
                ViewBag.Message = "Usage: /mute?duration={duration}" +
                                  "<br />" +
                                  "Ex: /mute?duration=3d" +
                                  "<br />" +
                                  "Ex: /mute?duration=forever";
                return View("Error");
            }

            TimeSpan timeDuration;
            try
            {
                timeDuration = UserDisciplinarian.ParseTimeString(duration);
            }
            catch
            {
                ViewBag.Message = "Invalid duration.";
                return View("Error");
            }
            if (timeDuration.TotalMinutes <= 0) return View("Error");

            var disciplinarian = new UserDisciplinarian(db);
            await disciplinarian.Mute(user, timeDuration);
            ViewBag.Message = "Muted " + user.UserName + " for " + duration + ".";
            return View("Success");
        }

        private async Task<ActionResult> Unban(CarolineRedisDb db, User user)
        {
            var disciplinarian = new UserDisciplinarian(db);
            await disciplinarian.Unban(user);
            ViewBag.Message = "Unbanned " + user.UserName + ".";
            return View("Success");
        }

        private async Task<ActionResult> Ban(CarolineRedisDb db, User user)
        {
            var duration = Request.QueryString["duration"];
            var message = Request.QueryString["message"];

            if (duration == null)
            {
                ViewBag.Message = "Usage: /ban?duration={duration}" +
                                  "<br />" +
                                  "Ex: /ban?duration=3d" +
                                  "<br />" +
                                  "Ex: /ban?duration=forever";
                return View("Error");
            }
            TimeSpan timeDuration;
            try
            {
                timeDuration = UserDisciplinarian.ParseTimeString(duration);
            }
            catch
            {
                ViewBag.Message = "Invalid duration.";
                return View("Error");
            }
            if (timeDuration.TotalMinutes <= 0) return View("Error");

            var disciplinarian = new UserDisciplinarian(db);
            await disciplinarian.Ban(user, timeDuration);

            ViewBag.Message = "Banned " + user.UserName + " for " + duration + ".";
            return View("Success");
        }

        private async Task<ActionResult> RemoveClaim(CarolineRedisDb db, User user)
        {
            var type = Request.QueryString["type"];
            var value = Request.QueryString["value"];

            if (type == null || value == null)
            {
                ViewBag.Message = "Usage: /remove-claim?type={type}&value={value}" +
                                  "<br />" +
                                  "Ex: /remove-claim?type=Authority&value=Administrator";
                return View("Error");
            }
            var authorizer = new UserAuthorizer(db);
            await authorizer.RemoveClaim(user, new UserClaim {ClaimType = type, ClaimValue = value});

            ViewBag.Message = "Removed claim " + type + " " + value + " to " + user.UserName + ".";
            return View("Success");
        }

        private async Task<ActionResult> AddClaim(CarolineRedisDb db, User user)
        {
            var type = Request.QueryString["type"];
            var value = Request.QueryString["value"];

            if (type == null || value == null)
            {
                ViewBag.Message = "Usage: /add-claim?type={type}&value={value}" +
                                  "<br />" +
                                  "Ex: /add-claim?type=Authority&value=Administrator";
                return View("Error");
            }
            var authorizer = new UserAuthorizer(db);
            await authorizer.AddClaim(user, new UserClaim {ClaimType = type, ClaimValue = value});

            ViewBag.Message = "Added claim " + type + " " + value + " to " + user.UserName + ".";
            return View("Success");
        }

        private async Task<ActionResult> DisplayUser(User user, CarolineRedisDb db)
        {
            var model = new UserAdministrationViewModel {UserName = user.UserName, Id = user.Id};
            foreach (var claim in user.Claims)
            {
                model.Claims.Add(new AdminstrationUserClaim
                {
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue
                });
            }
            foreach (var punishment in user.Punishments)
            {
                var expiryTime = new DateTime(punishment.PunishmentExpiry);
                var timeLeft = expiryTime - DateTime.UtcNow;
                model.Punishments.Add(new AdministrationUserPunishment
                {
                    Type = punishment.PunishmentType,
                    Expiry = expiryTime.ToLongDateString() + " " + expiryTime.ToLongTimeString(),
                    TimeRemaining =
                        timeLeft.Days + " days, " + timeLeft.Hours + " hours, " + timeLeft.Minutes + " minutes remaining"
                });
            }
            var disciplinarian = new UserDisciplinarian(db);
            model.IsBanned = await disciplinarian.IsBanned(user);
            model.IsMuted = await disciplinarian.IsMuted(user);
            model.IsAdministrator = UserAuthorizer.IsAdministrator(user);
           // var lastActive = new DateTime(user.LastActive);
           // model.LastActive = lastActive.ToLongTimeString() + " " + lastActive.ToShortDateString();
            return View(model);
        }

        private async Task<bool> IsAdministrator()
        {
            //var endpoint = IpEndpoint.TryParse(request.Environment);
            var userId = HttpContext.User.Identity.GetUserId<long>();
            if (userId == 0) return false;

            var db = await CarolineRedisDb.CreateAsync();
            var store = new RedisUserStore(db);
            var user = await store.FindByIdAsync(userId);

            return (UserAuthorizer.IsAdministrator(user) || user.UserName == "Hunter");
        }

        private async Task<User> GetUser(string username)
        {
            try
            {
                var db = await CarolineRedisDb.CreateAsync();
                var store = new RedisUserStore(db);
                var user = await store.FindByNameAsync(username);

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}