using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;


namespace Caroline.Persistence
{
    public class NotificationManager
    {
        private CarolineRedisDb _db;

        NotificationManager(CarolineRedisDb db)
        {
            _db = db;
        }

        public static async Task<NotificationManager> CreateAsync()
        {
            return new NotificationManager(await CarolineRedisDb.CreateAsync());
        }

        public async Task PushNotification(string message, string type)
        {
            var serverNotifications = await _db.ServerNotifications.Get("StupidHack") ?? await Create();
            serverNotifications.AddNotification(message,type);
            await _db.ServerNotifications.Set(serverNotifications);
        }

        public async Task RemoveNotification(string message, string type)
        {
            var serverNotifications = await _db.ServerNotifications.Get("StupidHack") ?? await Create();
            serverNotifications.RemoveNotification(message, type);
            await _db.ServerNotifications.Set(serverNotifications);
        }

        public async Task<List<ServerNotifications.Notification>>  GetNotifications()
        {
            var notifications = new List<ServerNotifications.Notification>();
            var serverNotifications = await _db.ServerNotifications.Get("StupidHack") ?? await Create();
            foreach (var notification in serverNotifications.Notifications)
                notifications.Add(new ServerNotifications.Notification() { Message = notification.Message, Type = notification.Type });

            return notifications;
        }

        private async Task<ServerNotifications> Create()
        {
            var serverNotifications = new ServerNotifications();
            await _db.ServerNotifications.Set(serverNotifications);
            return serverNotifications;
        }
    }
}
