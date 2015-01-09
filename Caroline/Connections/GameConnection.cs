using System.Threading.Tasks;
using System.Web;
using Caroline.Api;
using Caroline.App;
using Caroline.App.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;

namespace Caroline.Connections
{
    public class GameConnection : PersistentConnection
    {
        readonly GameManager _gameManager = new GameManager();
        
        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            var state = await _gameManager.Update(HttpContext.Current.User.Identity.GetUserId(), connectionId);
            await Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            var actions = ProtoBufHelpers.Deserialize<ClientActions>(data);

            if (actions.SocialActions != null) Socialize(request, actions);

            var state = await _gameManager.Update(HttpContext.Current.User.Identity.GetUserId(), connectionId, actions);
            await Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }

        protected override bool AuthorizeRequest(IRequest request)
        {
            // possibly extremely laggy
            return AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext());
        }

        private void Socialize(IRequest request, ClientActions actions)
        {
            foreach (var action in actions.SocialActions)
            {
                if (action.Chat != null)
                    if (action.Chat.GlobalMessage != null)
                        SendGlobalChatMessage(request.User.Identity.Name, action.Chat.GlobalMessage);
                    
            }
        }

        private void SendGlobalChatMessage(string sender, string text)
        {
            SendGlobalChatMessage(sender, text, DateTime.UtcNow.ToShortTimeString());
        }

        private void SendGlobalChatMessage(string sender, string text, string time)
        {
            string[] developers = new string[]{"Developer","Hunter"};
            string[] moderators = new string[] { "scrublord" };
            var maxMessageLength = 220;

            var state = new GameState();
            var message = new GameState.ChatMessage();
            message.Text = (text.Length > maxMessageLength) ? text.Substring(0, maxMessageLength) : text;
            message.Sender = sender;
            message.Time = time;
            if (Array.IndexOf(developers, sender) != -1) message.Permissions="developer";
            if (Array.IndexOf(moderators, sender) != -1) message.Permissions = "moderator";
            state.Message = message;

            Connection.Broadcast(ProtoBufHelpers.Serialize(state));
        }
    }
}