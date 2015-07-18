using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class ServerNotifications : IIdentifiableEntity<string>
    {
        public string Id
        {
            get { return "StupidHack"; }
            set { }
        }

        public void AddNotification(string message, string type)
        {
            Notifications.Add(new ServerNotifications.Notification { Message = message, Type = type });
        }

        public void RemoveNotification(string message, string type)
        {
            var toRemove = new List<ServerNotifications.Notification>();
            foreach (var notification in Notifications)
            {
                if (notification.Message == message && notification.Type == type)
                    toRemove.Add(notification);
            }

            foreach (var notification in toRemove)
            {
                Notifications.Remove(notification);
            }
        }
    }
}
