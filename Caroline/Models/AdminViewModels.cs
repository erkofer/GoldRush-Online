using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Caroline.App.Models;

namespace Caroline.Models
{
    public class NotificationsViewModel
    {
        public NotificationsViewModel()
        {
            Notifications = new List<Notification>();
        }

        public List<Notification> Notifications { get; set; } 
    }

    public class Notification
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }
}