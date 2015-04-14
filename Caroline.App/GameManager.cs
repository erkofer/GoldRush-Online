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

            // get connection session, check for rate limiting
            var session = await userDto.GetSession(endpoint.EndPoint);
            if (IsRateLimited(session))
                return new GameState { IsError = true, IsRateLimited = true };
            
            
            // get game save
            var save = await userDto.GetGame();
            var saveData = save.SaveData;
            var saveObject = saveData != null ? ProtoBufHelpers.Deserialize<SaveState>(saveData) : null;

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

            await manager.SetLeaderboardEntry(userId, updateDto.Score);

            // dispose lock on user, no more reading/saving
            await userDto.DisposeAsync();

            return updateDto != null ? updateDto.GameState : null;
        }

        static bool IsRateLimited(GameSession session)
        {
            RateLimit limit;
            if ((limit = session.RateLimit) == null)
                limit = session.RateLimit = new RateLimit();
            return !limit.TryRequest();
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null);
    }
}
