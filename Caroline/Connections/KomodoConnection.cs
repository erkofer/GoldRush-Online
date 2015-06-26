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
using Caroline.App.Models;

namespace Caroline.Connections
{
    public class KomodoConnection : PersistentConnection
    {
        readonly IGameManager _gameManager = new KomodoManager();

        static readonly List<string> UserIdList = new List<string>();   

        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            if(UserIdList.IndexOf(connectionId) == -1)
                UserIdList.Add(connectionId);

            await AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(request.GetHttpContext());
        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            await Update(request, connectionId, data);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            if (UserIdList.IndexOf(connectionId) > -1)
                UserIdList.Remove(connectionId);

            return base.OnDisconnected(request, connectionId, stopCalled);
        }

        async Task Update(IRequest request, string connectionId, string data = null)
        {
            var actions = data != null ? ProtoBufHelpers.Deserialize<ClientActions>(data) : null;
            var endpoint = IpEndpoint.TryParse(request.Environment);
            if (endpoint == null)
                throw new Exception("Can not get IP addresses from owin environment.");
            var userId = HttpContext.Current.User.Identity.GetUserId<long>();
            if (userId == 0)
                return;
            var gameEndpoint = new GameSessionEndpoint(endpoint, userId);
            
            var state = await _gameManager.Update(gameEndpoint, actions);
            state.ConnectedUsers = UserIdList.Count;
            await Connection.Send(connectionId, ProtoBufHelpers.SerializeToString(state));
        }
    }
}