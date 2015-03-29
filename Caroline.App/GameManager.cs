using System.Threading.Tasks;
using Caroline.App.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using GoldRush.APIs;

namespace Caroline.App
{
    public class GameManager : IGameManager
    {
        readonly KomodoSessionFactory _sessionFactory = new KomodoSessionFactory();
        readonly GoldRushCache _goldRushCache = new GoldRushCache();

        public async Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null)
        {
            var userId = endpoint.GameId;
            UpdateDto updateDto;
            var db = await CarolineRedisDb.CreateAsync();

            var games = db.Games;
            var userLocks = db.UserLocks;

            // lock the user and its games
            var user = new User { Id = userId };
            using (userLocks.Lock(user))
            {
                // get game save, create it if it doesn't exist
                Game save;
                save = await games.Get(new Game { Id = userId });
                if (save == null)
                    await games.Set(save = new Game { Id = userId });

                var saveData = save.SaveData;
                var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;
                var session = await db.GameSessions.Get(new GameSession { EndPoint = endpoint });

                // load game save into an game instance
                var game = _sessionFactory.Create();
                game.Load(new LoadArgs { SaveState = saveObject });

                // update save with new input
                updateDto = game.Update(new UpdateArgs { ClientActions = input, Session = session });

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

            }

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
