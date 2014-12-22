using System.Threading.Tasks;
using System.Web;
using Caroline.App;
using Caroline.App.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;

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
            var state = await _gameManager.Update(HttpContext.Current.User.Identity.GetUserId(), connectionId, actions);
            await Connection.Send(connectionId, ProtoBufHelpers.Serialize(state));
        }

        protected override bool AuthorizeRequest(IRequest request)
        {
            return request.User != null && request.User.Identity.IsAuthenticated;
        }
    }
}