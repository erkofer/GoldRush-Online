using System.Threading.Tasks;
using Caroline.App.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using GoldRush;

namespace Caroline.App
{
    public class GameManager : IGameManager
    {
        readonly GameFactory _gameFactory = new GameFactory();
        readonly GameStateCache _gameStateCache = new GameStateCache();

        public async Task<GameState> Update(string userId, string sessionGuid, ClientActions input = null)
        {
            GameState dataToSend;
            using (var work = new UnitOfWork())
            {
                var games = work.Games;

                // get game save, create it if it doesn't exist
                var save = await games.GetByUseridAsync(userId) ?? await games.Add(new Game());
                var saveData = save.SaveData;
                var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;

                // load game save into an game instance
                var game = _gameFactory.Create();
                game.Load(saveObject);

                // update save with new input
                dataToSend = game.Update(input, UpdateFlags.ReturnAllState);

                // save to the database
                save.SaveData = ProtoBufHelpers.Serialize(game.Save());
                games.Update(save);

                await work.SaveChangesAsync();
            }

            if(dataToSend == null)
                dataToSend = new GameState();

            // minify the GameState by only sending differences since the last state
            var previousState = _gameStateCache.GetGameData(sessionGuid);
            _gameStateCache.SetGameData(sessionGuid, dataToSend);
            // todo: build deep, minified copy by looking at differences between them via reflection
            return dataToSend;
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(string userId, string sessionGuid, ClientActions input = null);
    }
}
