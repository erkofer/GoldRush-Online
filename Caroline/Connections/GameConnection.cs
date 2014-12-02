using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.WebPages;
using GoldRush;
using Microsoft.AspNet.SignalR;

namespace Caroline.Connections
{
    public class GameConnection : PersistentConnection
    {
        readonly object _lock = new object();
        GameFactory _gameFactory = new GameFactory();
        readonly BiDictionary<string, IGoldRushGame> _gamesByConnectionId = new BiDictionary<string, IGoldRushGame>();
        readonly Dictionary<string, IGoldRushGame> _gamesByUserId = new Dictionary<string, IGoldRushGame>(100);

        protected override async Task OnConnected(IRequest request, string connectionId)
        {
            var id = UserIdProvider.GetUserId(request);
            if (id == null)
            {
                // not signed in, create anonymous credentials
                var resp = request.GetHttpContext().Response;
                resp.SetStatus(HttpStatusCode.Unauthorized);
                return;
            }

            IGoldRushGame game;
            lock (_lock)
            {
                // if the game exists for the same session (for some reason)
                if (_gamesByConnectionId.TryGetValue(connectionId, out game))
                {
                    return;
                }

                // if game exists for user, but on a different connection, disconnect old user
                _gamesByUserId.Remove(id);
                if (_gamesByUserId.TryGetValue(UserIdProvider.GetUserId(request), out game))
                {
                    _gamesByUserId.Remove(id);
                    _gamesByConnectionId.Reverse.Remove(game);
                }

                // game doesn't exist, create it
                game = _gameFactory.Create();
                _gamesByConnectionId.Add(connectionId, game);
                _gamesByUserId.Add(id, game);
            }
            // TODO: load game
        }

        protected override async Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            var id = UserIdProvider.GetUserId(request);
            lock (_lock)
            {
                IGoldRushGame game;
                if (id != null)
                    _gamesByUserId.Remove(id);
                _gamesByConnectionId.Remove(connectionId);
            }
        }

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            IGoldRushGame game;
            if (!_gamesByConnectionId.TryGetValue(connectionId, out game))
            {
                // game doesn't exist ?
                return;
            }

            await Connection.Send(connectionId, data);
        }

        protected override bool AuthorizeRequest(IRequest request)
        {
            return request.User != null && request.User.Identity.IsAuthenticated;
        }

        void CreateAnonymousCredentials(IRequest request)
        {

        }
    }
}