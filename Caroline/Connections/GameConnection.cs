using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using GoldRush;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using ProtoBuf;

namespace Caroline.Connections
{
    public class GameConnection : PersistentConnection
    {
        readonly GameFactory _gameFactory = new GameFactory();

        protected override async Task OnReceived(IRequest request, string connectionId, string data)
        {
            // verify identity
            var identity = HttpContext.Current.User.Identity;
            if (identity.IsAuthenticated)
            {
                var message = Serialize(new OutputState { Error = true });
                await Connection.Send(connectionId, message);
                return;
            }

            var userId = identity.GetUserId();
            OutputState dataToSend;
            using (var work = new UnitOfWork())
            {
                var games = work.Games;

                // get game save, create it if it doesn't exist
                var save = await games.GetByUseridAsync(userId) ?? await games.Add(new Game());
                var saveData = save.SaveData;
                var saveObject = saveData != null ? Deserialize<GameSave>(saveData) : null;

                // load game save into an game instance
                var game = _gameFactory.Create();
                game.Load(saveObject);

                // update save with new input
                dataToSend = game.Update(Deserialize<InputState>(data), UpdateFlags.ReturnAllState);

                // save to the database
                save.SaveData = Serialize(game.Save());
                games.Update(save);

                await work.SaveChangesAsync();
            }

            // send output to the client
            await Connection.Send(connectionId, Serialize(dataToSend)); // send new state to the client
        }

        protected override bool AuthorizeRequest(IRequest request)
        {
            return request.User != null && request.User.Identity.IsAuthenticated;
        }

        static T Deserialize<T>(string data)
        {
            var bytes = new byte[data.Length * sizeof(char)];
            Buffer.BlockCopy(data.ToCharArray(), 0, bytes, 0, bytes.Length);
            var stream = new MemoryStream(bytes, false);
            return Serializer.Deserialize<T>(stream);
        }

        static string Serialize<T>(T data)
        {
            // we have to serialize to an expandable memorystream then copy it to a byte[]
            // instead of serializing to a string directly
            var stream = new MemoryStream(4096);
            Serializer.Serialize(stream, data);
            var message = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Position);
            return message;
        }
    }
}