using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caroline.Domain;
using Caroline.Persistence.Models;
using GoldRush.APIs;
using Caroline.App.Models;
using Caroline.Persistence;

namespace Caroline.App
{
    public class KomodoManager : IGameManager
    {
        readonly KomodoSessionFactory _sessionFactory = new KomodoSessionFactory();

        public async Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null, bool clearSession = false)
        {
            var userId = endpoint.GameId;
            var manager = await UserManager.CreateAsync();
            var userDto = await manager.GetUser(userId);
            var db = await CarolineRedisDb.CreateAsync();

            // get connection session, check for rate limiting
            GameSession session;
            if (!clearSession)
                session = await userDto.GetSession(endpoint.EndPoint);
            else
            {
                session = new GameSession(new GameSessionEndpoint(endpoint.EndPoint, userId));
            }
            if (IsRateLimited(session))
                return new GameState { IsError = true, IsRateLimited = true };

            var user = await userDto.GetUser();
            /* If the user has not already been tagged
             * as an alpha player then we'll tag them now.
             * Remove this when we exit alpha.
             */
            if (!UserAuthorizer.IsAlphaVeteran(user))
            {
                var authorizer = new UserAuthorizer(db);
                await authorizer.AddClaim(user, new UserClaim { ClaimType = "Veteran", ClaimValue = "Alpha" });
            }

            // get game save
            var save = await userDto.GetGame();
            var chat = await ChatManager.CreateAsync();

            // load game save into an game instance
            var game = _sessionFactory.Create();
            game.Load(new LoadArgs { SaveState = save });

            var marketPlace = new MarketPlaceGlue(await MarketPlace.CreateAsync());

            // update save with new input
            var updateDto = await game.Update(new UpdateArgs { ClientActions = input, Session = session, MarketPlace = marketPlace, User = user });

            var errors = await SendMessages(user, input, chat);
            var messages = await chat.GetRecentMessages(session.LastChatMessageRecieved);
            updateDto.GameState.Messages.AddRange(errors);
            updateDto.GameState.Messages.AddRange(messages.Item1);

            session.LastChatMessageRecieved = messages.Item2;

            // save to the database
            var saveDto = game.Save();
            saveDto.SaveState.Id = save.Id;
            // session gets modified by update
            await userDto.SetSession(session);
            saveDto.SaveState.Id = userId;
            await userDto.SetGame(saveDto.SaveState);

            if (!user.IsAnonymous)
                await manager.SetLeaderboardEntry(userId, updateDto.Score);

            // dispose lock on user, no more reading/saving
            await userDto.DisposeAsync();
            // permissions, text, sender, time

            return updateDto.GameState;
        }

        static bool IsRateLimited(GameSession session)
        {
            RateLimit limit;
            if ((limit = session.RateLimit) == null)
                limit = session.RateLimit = new RateLimit();
            return !limit.TryRequest();
        }

        // todo: move these chat helper methods to ChatManager
        async Task<List<GameState.ChatMessage>> SendMessages(User user, ClientActions actions, ChatManager manager)
        {
            if (actions == null || actions.SocialActions == null || actions.SocialActions.Count == 0)
                return new List<GameState.ChatMessage>();

            var ret = new List<GameState.ChatMessage>();
            var db = await CarolineRedisDb.CreateAsync();
            var disciplinarian = new UserDisciplinarian(db);
            for (var i = 0; i < actions.SocialActions.Count; i++)
            {
                var action = actions.SocialActions[i];
                if (action.Chat == null)
                    continue;
                if (action.Chat.GlobalMessage == null)
                    continue;

                if (!user.IsAnonymous)
                {
                    if (!await disciplinarian.IsMuted(user))
                        await manager.SendMessage(action.Chat.GlobalMessage, user);
                    else
                    {
                        var completionTime = disciplinarian.GetMuteCompletionTime(user);
                        ret.Add(BuildServerMessage("You are muted until " + completionTime.ToShortDateString() + " " + completionTime.ToShortTimeString() + "."));
                    }
                }
                else
                    ret.Add(BuildServerMessage("You must be registered to send chat messages."));
            }
            return ret;
        }

        GameState.ChatMessage BuildServerMessage(string text)
        {
            return new GameState.ChatMessage
            {
                Text = text,
                Sender = "SERVER",
                Permissions = "server",
                Time = DateTime.UtcNow.ToShortTimeString()
            };
        }
    }

    public interface IGameManager
    {
        Task<GameState> Update(GameSessionEndpoint endpoint, ClientActions input = null, bool clearSession=false);
    }
}
