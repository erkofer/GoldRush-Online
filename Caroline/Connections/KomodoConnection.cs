using System.Threading.Tasks;
using System.Web;
using Caroline.Api;
using Caroline.App;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caroline.App.Models;

namespace Caroline.Connections
{
    public class KomodoConnection : PersistentConnection
    {
        readonly IGameManager _gameManager = new KomodoManager();

        static readonly List<string> UserIdList = new List<string>();
        static readonly List<string> JustJoinedIdList = new List<string>();
        static Random random = new Random();

        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            if (UserIdList.IndexOf(connectionId) == -1)
                UserIdList.Add(connectionId);

            if (JustJoinedIdList.IndexOf(connectionId) == -1)
                JustJoinedIdList.Add(connectionId);

            await AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext());

            var userId = HttpContext.Current.User.Identity.GetUserId<long>();
            if (userId == 0)
                return;


        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            // clear sessions for users who have just connected.
            if (JustJoinedIdList.IndexOf(connectionId) > -1)
            {
                JustJoinedIdList.Remove(connectionId);
                await Update(request, connectionId, data, true);
            }
            else
                await Update(request, connectionId, data);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            if (UserIdList.IndexOf(connectionId) > -1)
                UserIdList.Remove(connectionId);

            return base.OnDisconnected(request, connectionId, stopCalled);
        }

        async Task Update(IRequest request, string connectionId, string data = null, bool clearSession = false)
        {
            var actions = data != null ? ProtoBufHelpers.Deserialize<ClientActions>(data) : null;
            var endpoint = IpEndpoint.TryParse(request.Environment);
            if (endpoint == null)
                throw new Exception("Can not get IP addresses from owin environment.");

            var userHasSessionId = false;
            if (actions.SessionId == null)
                actions.SessionId = random.Next(1, 9999).ToString();
            else
                userHasSessionId = true;

            endpoint.RemotePort = actions.SessionId;

            var userId = HttpContext.Current.User.Identity.GetUserId<long>();
            if (userId == 0)
                return;
            var gameEndpoint = new GameSessionEndpoint(endpoint, userId);
            var state = await _gameManager.Update(gameEndpoint, actions, clearSession);
            // send the user their session id if they don't already have it.
            if(!userHasSessionId)
                state.SessionId = actions.SessionId;

            state.ConnectedUsers = UserIdList.Count;
            await Connection.Send(connectionId, ProtoBufHelpers.SerializeToString(state));
        }
    }

    public static class Hacks
    {
        public static string GetProductVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return attribute;
        }
    }
}