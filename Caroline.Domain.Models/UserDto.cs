using System;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;

namespace Caroline.Domain.Models
{
    public class UserDto : DtoBase
    {
        readonly CarolineRedisDb _db;
        readonly long _id;

        public UserDto(IAsyncDisposable ulock, CarolineRedisDb db, long id)
            : base(ulock)
        {
            _db = db;
            _id = id;
        }

        public Task<User> GetUser()
        {
            Check();
            return _db.Users.Get(_id);
        }

        public Task<bool> SetUser(User entity)
        {
            CheckEqual(_id, entity);
            return _db.Users.Set(entity);
        }

        public async Task<Game> GetGame()
        {
            Check();
            return await _db.Games.Get(_id) ?? new Game { Id = _id };
        }
        // When we retire games, have a RetireGame(Game newGame = null) method that saves old games statistics and then deletes it.

        public Task SetGame(Game game)
        {
            CheckEqual(_id, game);
            return _db.Games.Set(game);
        }

        public async Task<GameSession> GetSession(IpEndpoint id)
        {
            return await _db.GameSessions.Get(new GameSessionEndpoint(id, _id)) ?? new GameSession(new GameSessionEndpoint(id, _id));
        }

        public Task<bool> SetSession(GameSession entity)
        {
            Check(entity);
            if (entity.Id.GameId != _id)
                throw new ArgumentException("entity GameId does not match this users id!", "entity");
            return _db.GameSessions.Set(entity);
        }
    }
}