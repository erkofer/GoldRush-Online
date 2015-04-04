using System.Threading.Tasks;
using System.Web;
using Caroline.Api;
using Caroline.App;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using Caroline.App.Models;

namespace Caroline.Connections
{
    public class GameConnection : PersistentConnection
    {
        readonly IGameManager _gameManager = new GameManager();

        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            await AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext());

            await Update(request, connectionId);
        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            var actions = ProtoBufHelpers.Deserialize<ClientActions>(data);
            if (actions.SocialActions != null) await Socialize(request, connectionId, actions);

            await Update(request, connectionId);
        }

        async Task Update(IRequest request, string connectionId)
        {
            IpEndpoint endpoint;
            if (!IpEndpoint.TryParse(request.Environment, out endpoint))
                throw new Exception("Can not get IP addresses from owin environment.");
            var userId = HttpContext.Current.User.Identity.GetUserId<long>();
            var gameEndpoint = new GameSessionEndpoint(endpoint, userId);

            var state = await _gameManager.Update(gameEndpoint);
            if (state != null)
                await Connection.Send(connectionId, ProtoBufHelpers.SerializeToString(state));
        }

        private async Task Socialize(IRequest request, string connectionId, ClientActions actions)
        {
            foreach (var action in actions.SocialActions)
            {
                if (action.Chat != null)
                    if (action.Chat.GlobalMessage != null)
                    {
                        var user = await GetUserName(request.GetHttpContext().User.Identity.GetUserId<long>());
                        if (!user.IsAnonymous)
                            SendGlobalChatMessage(user.UserName, action.Chat.GlobalMessage);
                        else
                            SendServerMessage(connectionId, "You must be registered to send chat messages.");
                    }

            }
        }

        async Task<User> GetUserName(long userId)
        {
            var db = await CarolineRedisDb.CreateAsync();
            return await db.Users.Get(userId);

        }

        private void SendGlobalChatMessage(string sender, string text)
        {
            SendGlobalChatMessage(sender, text, DateTime.UtcNow.ToShortTimeString());
        }

        private void SendGlobalChatMessage(string sender, string text, string time)
        {
            string[] developers = new string[] { "Developer", "Hunter" };
            string[] moderators = new string[] { "scrublord" };
            string[] server = new string[] { "Server" };
            var maxMessageLength = 220;

            var state = new GameState();
            var message = new GameState.ChatMessage();
            message.Text = (text.Length > maxMessageLength) ? text.Substring(0, maxMessageLength) : text;
            message.Sender = sender;
            message.Time = time;
            if (Array.IndexOf(developers, sender) != -1) message.Permissions = "developer";
            if (Array.IndexOf(moderators, sender) != -1) message.Permissions = "moderator";
            if (Array.IndexOf(server, sender) != -1) message.Permissions = "server";
            state.Messages.Add(message);
            // TODO: Consider changing to allow message batching.
            Connection.Broadcast(ProtoBufHelpers.SerializeToString(state));
        }

        private void SendServerMessage(string connectionId, string text)
        {
            var state = new GameState();
            var message = new GameState.ChatMessage();
            message.Text = text;
            message.Sender = "SERVER";
            message.Permissions = "server";
            message.Time = DateTime.UtcNow.ToShortTimeString();
            state.Messages.Add(message);

            Connection.Send(connectionId, ProtoBufHelpers.SerializeToString(state));
        }
    }
}