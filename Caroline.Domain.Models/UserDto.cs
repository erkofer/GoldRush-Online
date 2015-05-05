using System;
using System.Threading.Tasks;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;

namespace Caroline.Domain.Models
{
    public class UserDto : DtoBase
    {
        readonly CarolineRedisDb _redis;
        readonly CarolineMongoDb _mongo;
        readonly long _id;

        public UserDto(IAsyncDisposable ulock, CarolineRedisDb redis, CarolineMongoDb mongo, long id)
            : base(ulock)
        {
            _redis = redis;
            _mongo = mongo;
            _id = id;
        }

        public Task<User> GetUser()
        {
            Check();
            return _redis.Users.Get(_id);
        }

        public Task<bool> SetUser(User entity)
        {
            CheckEqual(_id, entity);
            return _redis.Users.Set(entity);
        }

        public async Task<SaveState> GetGame()
        {
            Check();
            return await _redis.Games.Get(_id) ?? new SaveState { Id = _id };
        }
        // When we retire games, have a RetireGame(Game newGame = null) method that saves old games statistics and then deletes it.

        public Task SetGame(SaveState game)
        {
            CheckEqual(_id, game);
            return _redis.Games.Set(game);
        }

        public async Task<GameSession> GetSession(IpEndpoint id)
        {
            return await _redis.GameSessions.Get(new GameSessionEndpoint(id, _id)) ?? new GameSession(new GameSessionEndpoint(id, _id));
        }

        public Task<bool> SetSession(GameSession entity)
        {
            Check(entity);
            if (entity.Id.GameId != _id)
                throw new ArgumentException("entity GameId does not match this users id!", "entity");
            return _redis.GameSessions.Set(entity);
        }
    }
}