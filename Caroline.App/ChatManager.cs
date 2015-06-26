using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using StackExchange.Redis;

namespace Caroline.App
{
    public class ChatManager
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        const string PublicChatroom = "pub:pub";
        //readonly ChatroomManager _rooms;

        ChatManager(CarolineRedisDb db)
        {
            _db = db;
        }

        public static async Task<ChatManager> CreateAsync()
        {
            return new ChatManager(await CarolineRedisDb.CreateAsync());
        }

        public Task SendMessage(string message, User sender)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Task.FromResult(0);
            var dto = new ChatroomMessage
            {
                Id = PublicChatroom,
                Message = message,
                UserName = sender.UserName,
                UserId = sender.Id,
                Permissions = GetPermissions(sender.UserName),
                Time = DateTime.UtcNow.ToShortTimeString()
            };
            return _db.ChatroomMessages.Push(dto, IndexSide.Right);
            //var permissions = GetPermissions(sender.UserName);
            //switch (await _rooms.SendMessage(message, PublicChatroom, sender, permissions, false))
            //{
            //    case SendMessageResult.Success:
            //        break;
            //    case SendMessageResult.BadArguments:
            //        break;
            //    case SendMessageResult.NotSubscribed:
            //        Log.Warn("SendMessage.NotSubscribed result from sending a public message.");
            //        break;
            //    case SendMessageResult.BlankMessage:
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        public async Task<Tuple<GameState.ChatMessage[], long>> GetRecentMessages(long? lastMessageRecieved = null)
        {
            const long maxMessagesReturned = 10;
            var result = await _db.ChatroomMessages.RangeByScore(PublicChatroom, double.PositiveInfinity, double.NegativeInfinity, Exclude.Stop, Order.Descending, 0, maxMessagesReturned);

            long take;
            if (lastMessageRecieved != null)
            {
                if (result.Length > 0)
                    take = Math.Max(Math.Min((long)result[0].Index - lastMessageRecieved.Value, maxMessagesReturned), 0);
                else
                    return new Tuple<GameState.ChatMessage[], long>(new GameState.ChatMessage[0], 0);
            }
            else take = maxMessagesReturned;
            var ret = new GameState.ChatMessage[take];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new GameState.ChatMessage
                {
                    Permissions = result[i].Permissions,
                    Sender = result[i].UserName,
                    Text = result[i].Message,
                    Time = result[i].Time
                };
            }

            Array.Reverse(ret);

            long index = 0;
            if (result.Length > 0)
                index = (long)result[0].Index;
            return new Tuple<GameState.ChatMessage[], long>(ret, index);
        }

        static readonly Dictionary<string, string> Permissions = new Dictionary<string, string>
        {
            {"Tristyn", "developer"},
            {"Hunter", "developer"},
            {"Ell dubs", "moderator"},
            {"Server", "server"}
        };

        CarolineRedisDb _db;

        string GetPermissions(string sender)
        {
            string permission;
            return Permissions.TryGetValue(sender, out permission) ? permission : string.Empty;
        }
    }
}
