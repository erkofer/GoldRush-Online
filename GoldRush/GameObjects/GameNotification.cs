using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    public class GameNotification
    {
        public GameNotification(string message, string tag)
        {
            Message = message;
            Tag = tag;
        }

        public string Message;
        public string Tag;
    }

    public class GameNotificationEventArgs : EventArgs
    {
        public GameNotification Notification;
    }

    public delegate void GameNotificationEventHandler(object sender, GameNotificationEventArgs e);

    interface INotifier
    {
        event GameNotificationEventHandler GameNotification;
    }
}
