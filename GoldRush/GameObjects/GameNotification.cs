using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class GameNotification
    {
        public string Message;
        public string Tag;
    }

    public class GameNotificationEventArgs : EventArgs
    {
        public string Message;
        public string Tag;
    }

    public delegate void GameNotificationEventHandler(object sender, GameNotificationEventArgs e);
}
