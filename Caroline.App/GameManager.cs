﻿using System.Threading.Tasks;
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
            var sessionId = new GameSession { EndPoint = endpoint };
            var session = await db.GameSessions.Get(sessionId);

                // load game save into an game instance
            var game = _sessionFactory.Create();
            game.Load(new LoadArgs { SaveState = saveObject });

                // update save with new input
            var updateDto = game.Update(new UpdateArgs { ClientActions = input, Session = session });

                // save to the database
            // session gets modified by update
            if (session != null)
                await db.GameSessions.Set(session);
            else await db.GameSessions.Delete(sessionId);
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
