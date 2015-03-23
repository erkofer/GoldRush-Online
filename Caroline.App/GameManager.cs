using System.Threading.Tasks;
using Caroline.App.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;
using GoldRush;

namespace Caroline.App
{
    public class GameManager : IGameManager
    {
        readonly GameFactory _gameFactory = new GameFactory();
        readonly GoldRushCache _goldRushCache = new GoldRushCache();

        public async Task<GameState> Update(long userId, string sessionGuid, ClientActions input = null)
        {
            GameState dataToSend;
            var db = await CarolineRedisDb.CreateAsync();

            var games = db.Games;
            // get game save, create it if it doesn't exist
            Game save;
            save = await games.Get(new Game { Id = userId });
            if (save == null)
                await games.Set(save = new Game { Id = userId });

            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;

            // load game save into an game instance
            var game = _gameFactory.Create();
            game.Load(saveObject);

            // update save with new input
            dataToSend = game.Update(input);

            // save to the database
            var saveDto = game.Save();
            save.SaveData = ProtoBufHelpers.SerializeToString(saveDto);
            await games.Set(save);

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
        Task<GameState> Update(long userId, string sessionGuid, ClientActions input = null);
    }
}
