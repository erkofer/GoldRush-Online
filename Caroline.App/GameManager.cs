using System.Diagnostics;
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
        readonly GoldRushCache _goldRushCache = new GoldRushCache();

        public async Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null)
        {
            var userId = endpoint.GameId;
            var db = await CarolineRedisDb.CreateAsync();

            var games = db.Games;

            // lock the user and its games
            var user = new User { Id = userId };
            var userLock = await db.UserLocks.Lock(user);

            // get game save, if it doesn't exist then use a new game
            Game save;
            save = await games.Get(save = new Game { Id = userId }) ?? save;

            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;
            var sessionId = new GameSession(endpoint);
            var session = await db.GameSessions.Get(sessionId) ?? sessionId;
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
                if (!await games.Set(save))
                    Debug.Assert(false);
            }

            // dispose lock on user, no more reading/saving
            await userLock.DisposeAsync();

            GameState dataToSend = null;
            if (updateDto != null)
                dataToSend = updateDto.GameState;
            if (dataToSend == null)
                dataToSend = new GameState();

            // minify the GameState by only sending differences since the last state
            //var previousState = _goldRushCache.GetGameData(sessionGuid);
            //_goldRushCache.SetGameData(sessionGuid, dataToSend);
            //if (previousState != null)
            //    dataToSend = dataToSend.Compress(previousState);
            return dataToSend;
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null);
    }
}
