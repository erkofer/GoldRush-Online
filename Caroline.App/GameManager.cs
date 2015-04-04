using System.Threading.Tasks;
using Caroline.Persistence;
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
            var db = await CarolineRedisDb.CreateAsync();

            var games = db.Games;

            // lock the user and its games
            var userLock = await db.UserLocks.Lock(userId);

            // get game save, if it doesn't exist then use a new game
            var save = await games.Get(userId) ?? new Game();

            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;
            var session = await db.GameSessions.Get(endpoint) ?? new GameSession();
            // load game save into an game instance
            var game = _sessionFactory.Create();
            game.Load(new LoadArgs { SaveState = saveObject });

            // update save with new input
            var updateDto = game.Update(new UpdateArgs { ClientActions = input, Session = session });

            // save to the database
            // session gets modified by update
            await db.GameSessions.Set(session);

            var saveDto = game.Save();
            var saveState = saveDto.SaveState;
            if (saveState != null)
            {
                // TODO: dont serialize twice
                save.SaveData = ProtoBufHelpers.SerializeToString(saveState);
                await games.Set(save);
            }

            // dispose lock on user, no more reading/saving
            await userLock.DisposeAsync();

            return updateDto != null ? updateDto.GameState : null;
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null);
    }
}
