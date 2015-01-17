using System;
using System.Threading.Tasks;
using Caroline.App.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using GoldRush;

namespace Caroline.App
{
    public class GameManager : DisposableBase, IGameManager
    {
        readonly IUnitOfWork _work;
        readonly GameFactory _gameFactory;
        readonly IGoldRushCache _goldRushCache;

        public GameManager(IUnitOfWork work, GameFactory gameFactory, IGoldRushCache cache)
        {
            _work = work;
            _gameFactory = gameFactory;
            _goldRushCache = cache;
        }

        public async Task<GameState> Update(string userId, string sessionGuid, ClientActions input = null)
        {
            GuardDispose();

            var games = _work.Games;

            // get game save, create it if it doesn't exist
            var save = await games.GetByUseridAsync(userId) ?? await games.AddByUserIdAsync(userId, new Game());
            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;

            // load game save into an game instance
            var game = _gameFactory.Create();
            game.Load(saveObject);

            // update save with new input
            var dataToSend = game.Update(input);

            // save to the database
            var saveDto = game.Save();
            save.SaveData = ProtoBufHelpers.Serialize(saveDto);
            games.Update(save);

            await _work.SaveChangesAsync();

            if (dataToSend == null)
                dataToSend = new GameState();

            // minify the GameState by only sending differences since the last state
            var previousState = _goldRushCache.GetGameData(sessionGuid);
            _goldRushCache.SetGameData(sessionGuid, dataToSend);
            if (previousState != null)
                dataToSend = dataToSend.Compress(previousState);
            return dataToSend;
        }

        protected override void DisposeObj()
        {
            _work.Dispose();
        }
    }



    public interface IGameManager : IDisposable
    {
        Task<GameState> Update(string userId, string sessionGuid, ClientActions input = null);
    }
}
