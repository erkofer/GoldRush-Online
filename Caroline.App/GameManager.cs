using System.Threading.Tasks;
using Caroline.Domain;
using Caroline.Persistence.Models;
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

            // load game save into an game instance
            var game = _sessionFactory.Create();
            game.Load(new LoadArgs { SaveState = save });

            // update save with new input
            var updateDto = game.Update(new UpdateArgs { ClientActions = input, Session = session });

            // save to the database
            var saveDto = game.Save();
            // session gets modified by update
            await userDto.SetSession(session);
            await userDto.SetGame(saveDto.SaveState);
            await manager.SetLeaderboardEntry(userId, updateDto.Score);

            // dispose lock on user, no more reading/saving
            await userDto.DisposeAsync();

            return updateDto.GameState;
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
