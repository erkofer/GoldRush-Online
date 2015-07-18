using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caroline.Models;
using Caroline.Persistence;
using Newtonsoft.Json;

namespace Caroline.Controllers
{
    public class ApiController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var data = new {Message="Error"};
            return Json(data);
        }

        public async Task<ActionResult> Notifications()
        {
            var notificationManager = await NotificationManager.CreateAsync();
            var notificationsModel = new NotificationsViewModel();
            var notifications = await notificationManager.GetNotifications();

            foreach (var notification in notifications)
                notificationsModel.Notifications.Add(new Notification()
                {
                    Message = notification.Message,
                    Type = notification.Type
                });

            return Json(JsonConvert.SerializeObject(notificationsModel), JsonRequestBehavior.AllowGet);
        }
    }
}