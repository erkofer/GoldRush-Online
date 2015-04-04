using System.Threading.Tasks;
using Caroline.Domain;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using GoldRush.APIs;
using Caroline.App.Models;

namespace Caroline.App
{
    public class GameManager : IGameManager
    {
        readonly KomodoSessionFactory _sessionFactory = new KomodoSessionFactory();

        public async Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null)
        {
            var userId = endpoint.GameId;
            var manager = await UserManager.CreateAsync();
            var userDto = await manager.GetUser(userId);

            // get game save and connection session
            var save = await userDto.GetGame();
            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;
            var session = await userDto.GetSession(endpoint.EndPoint);

            // load game save into an game instance
            var game = _sessionFactory.Create();
            game.Load(new LoadArgs { SaveState = saveObject });

            // update save with new input
            var updateDto = game.Update(new UpdateArgs { ClientActions = input, Session = session });

            // save to the database
            // session gets modified by update
            await userDto.SetSession(session);

            var saveDto = game.Save();
            var saveState = saveDto.SaveState;
            if (saveState != null)
            {
                // TODO: dont serialize twice
                save.SaveData = ProtoBufHelpers.SerializeToString(saveState);
                await userDto.SetGame(save);
            }

            // dispose lock on user, no more reading/saving
            await userDto.DisposeAsync();

            return updateDto != null ? updateDto.GameState : null;
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null);
    }
}
