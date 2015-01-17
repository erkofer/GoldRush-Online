using System.Threading.Tasks;
using System.Web;
using Caroline.Api;
using Caroline.App;
using Caroline.App.Models;
using Caroline.Areas.Api.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;

namespace Caroline.Connections
{
    public class GameConnection : PersistentConnection
    {
        readonly GameManager _gameManager;

        public GameConnection(GameManager games)
        {
            _gameManager = games;
        }
        
        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            var state = await _gameManager.Update(HttpContext.Current.User.Identity.GetUserId(), connectionId);
            await Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            var actions = ProtoBufHelpers.Deserialize<ClientActions>(data);

            if (actions.SocialActions != null) await Socialize(request, connectionId, actions);

            var state = await _gameManager.Update(HttpContext.Current.User.Identity.GetUserId(), connectionId, actions);
            await Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }

        protected override bool AuthorizeRequest(IRequest request)
        {
            // possibly extremely laggy
            return AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext());
        }

        private async Task Socialize(IRequest request,string connectionId, ClientActions actions)
        {
            foreach (var action in actions.SocialActions)
            {
                if (action.Chat != null)
                    if (action.Chat.GlobalMessage != null)
                    {
                        var user = await GetUserName(request.GetHttpContext().User.Identity.GetUserId());
                        if (!user.IsAnonymous)
                            SendGlobalChatMessage(user.UserName, action.Chat.GlobalMessage);
                        else
                            SendServerMessage(connectionId, "You must be registered to send chat messages.");
                    }

            }
        }

        async Task<ApplicationUser> GetUserName(string userId)
        {
            var account = new AccountViewModel();
                    
            using (var work = new UnitOfWork())
            {
                return await work.Users.Get(userId);
            }
        }

        private void SendGlobalChatMessage(string sender, string text)
        {
            SendGlobalChatMessage(sender, text, DateTime.UtcNow.ToShortTimeString());
        }

        private void SendGlobalChatMessage(string sender, string text, string time)
        {
            string[] developers = new string[] { "Developer", "Hunter" };
            string[] moderators = new string[] { "scrublord" };
            string[] server = new string[] {"Server"};
            var maxMessageLength = 220;

            var state = new GameState();
            var message = new GameState.ChatMessage();
            message.Text = (text.Length > maxMessageLength) ? text.Substring(0, maxMessageLength) : text;
            message.Sender = sender;
            message.Time = time;
            if (Array.IndexOf(developers, sender) != -1) message.Permissions = "developer";
            if (Array.IndexOf(moderators, sender) != -1) message.Permissions = "moderator";
            if (Array.IndexOf(server, sender) != -1) message.Permissions = "server";
            state.Message = message;

            Connection.Broadcast(ProtoBufHelpers.Serialize(state));
        }

        private void SendServerMessage(string connectionId, string text)
        {
            var state = new GameState();
            var message = new GameState.ChatMessage();
            message.Text = text;
            message.Sender = "SERVER";
            message.Permissions = "server";
            message.Time = DateTime.UtcNow.ToShortTimeString();
            state.Message = message;

            Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }
    }
}