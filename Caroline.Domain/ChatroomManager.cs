using System;
using System.Threading.Tasks;
using System.Web.Configuration;
using Caroline.Domain.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;

namespace Caroline.Domain
{
    public class ChatroomManager : IChatroomManager
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly long TransientChatroomMaxMessages;
        private static readonly TimeSpan transientChatroomSecondsToLive;
        const long ChatroomMessageCapacity = 25;
        const int MaxMessageLength = 200;

        readonly CarolineRedisDb _db;

        static ChatroomManager()
        {
            long messages = 25;
            var messagesString = WebConfigurationManager.AppSettings["transientChatroomMaxMessages"];
            if (messagesString != null)
            {
                if (!long.TryParse(messagesString, out messages) || messages <= 0)
                    throw new Exception("transientChatroomMaxMessages in Web.config must be a whole number greater than 0.");
            }
            TransientChatroomMaxMessages = messages;

            var secondsToLive = TimeSpan.FromSeconds(600);
            var secondsToLiveString = WebConfigurationManager.AppSettings["transientChatroomSecondsToLive"];
            if (secondsToLiveString != null)
            {
                long secondsToLiveLong;
                if (!long.TryParse(secondsToLiveString, out secondsToLiveLong) || secondsToLiveLong <= 0)
                    throw new Exception("transientChatroomSecondsToLive in Web.config must be a whole number greater than 0.");
                secondsToLive = TimeSpan.FromSeconds(secondsToLiveLong);
            }
            transientChatroomSecondsToLive = secondsToLive;
        }

        ChatroomManager(CarolineRedisDb db)
        {
            _db = db;
        }

        public static async Task<ChatroomManager> CreateAsync()
        {
            return new ChatroomManager(await CarolineRedisDb.CreateAsync());
        }

        public async Task<SendMessageResult> SendMessage(string message, string chatroom, User sender, string permissions, bool messagesExpire = true)
        {
            if (sender == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(message))
                return SendMessageResult.BlankMessage;

            var options = await _db.ChatroomOptions.Get(chatroom);
            if (options != null && options.IsPrivate)
                if (await _db.ChatroomSubscribers.Get(chatroom, sender.Id) == null)
                    return SendMessageResult.NotSubscribed;

            if (message.Length > MaxMessageLength)
                message = message.Substring(0, MaxMessageLength);
            // todo: chat swear filtering and shitty fuck tits

            // update message list in chatroom
            var messageIndex = await _db.ChatroomMessagesIdIncrement.IncrementAsync(chatroom);
            var messageDto = new ChatroomMessage { Id = chatroom, Message = message, UserName = sender.UserName, UserId = sender.Id, Index = messageIndex, Permissions = permissions, Time = DateTime.UtcNow.ToShortTimeString() };
            if (!await _db.ChatroomMessages.Add(messageDto))
            {
                // an existing message was overwritten, should never happen
                Log.Warn("Chatroom message with a unique id was overwritten!");
            }

            if (messagesExpire)
                // trim messages
                await _db.ChatroomMessages.RemoveRangeByRank(messageDto.Id, 0, -TransientChatroomMaxMessages);

            /* TODO: chat overhaul: changes to make chatrooms with many users (eg: public chat) scale better
                 * Instead of a capped per-user List<ChatroomNotifications>, change to a uncapped per-session List<ChatroomId> ChatroomsWithModifications
                 * and a uncapped per-chatroom List<UserId> UsersThatAreUpToDate.
                 * An entry in ChatroomsWithModifications denotes new messages in that chatroom.
                 * An entry in UsersThatAreUpToDate denotes users that should be notified when the next message arrives.
                 * 
                 * When a message arrives: RPopLPush from SessionsThatAreUpToDate onto ChatroomsWithModifications
                 * -- note that UsersThatAreUpToDate is a userId long and ChatroomsWithModifications is a chatroomId string
                 * -- maybe in both lists have an object with both user id and chatroom id to take advantage of RPopLPush
                 * 
                 * When a user requests message updates: RPopLPush from ChatroomsWithModifications to SessionsThatAreUpToDate
                 * then get new messages for that chatroom. 
                 * LastMessageRecievedId for each chatroom should be contained in the session, new sessions will get most recent 25-ish.
                 */
            //var tempId = Guid.NewGuid() + listenersList.Name;
            //db.TempChatroomNotificationListenerLists.Add(TempChatoroomNotificaitonListeners, tempId, score: DateTime.UtcNow); // use redis script to get time
            //db.MessageLogNotification.Rename(ChatNotificationListeners, TempNotificationListeners, list, tempId);
            //db.MessageLogNotification.Range(ChatNotificationListeners, tempId, double.NegativeInfinity, double.PositiveInfinity);
            //// foreach
            //db.MessageLogNotification.ListRightPopLeftPushAsync(TempNotificationListeners, tempId, userChatroomNotifications, userFromTempList);

            //db.TempChatroomNotificationListenerLists.Remove(TempChatoromoNotificaitonListeners, tempId);
            // TODO: run a job that monitors TempChatroomNotificationListenerLists counts with low scores (long running, possibly crashed).
            
            // message fanout
            var subscribers = await _db.ChatroomSubscribers.GetAll(chatroom);
            for (var i = 0; i < subscribers.Length; i++)
            {
                
                var subscriber = subscribers[i];
                var numNotifications = await _db.UserChatroomNotifications.Push(new ChatroomNotification
                {
                    Id = subscriber.UserId,
                    ChatroomId = chatroom,
                    Message = message,
                    SenderUserId = sender.Id
                }, IndexSide.Left);
                
                if (numNotifications < ChatroomMessageCapacity * 2)
                    continue;

                // we dont store older notifications, as we can generate it by looking at what chatrooms the user is subscribed to
                var numToRemove = numNotifications - ChatroomMessageCapacity;
                await _db.UserChatroomNotifications.Pop(numNotifications - ChatroomMessageCapacity, IndexSide.Right, numToRemove);
                 }

             return SendMessageResult.Success;
        }
        
        public Task<long> GetChatroomMessageIndex(string chatroom)
        {
            return _db.ChatroomMessagesIdIncrement.Get(chatroom);
        }

        public async Task<GameState.ChatMessage[]> GetRecentMessages(string chatroom, long lastMessageRecieved)
        {
            //if(start < 0)
            //    throw new ArgumentException("start must be equal to or greater than 0.", "start");
            //if(count <= 0)
            //    throw new ArgumentException("count must be greater than 0", "count");
            if (lastMessageRecieved < 0)
                throw new ArgumentException("lastMessageRecieved must be equal to or greater than 0.",
                    "lastMessageRecieved");

            var messages = await _db.ChatroomMessages.Range(chatroom, lastMessageRecieved);
            var ret = new GameState.ChatMessage[messages.Length];
            for (var i = 0; i < ret.Length; i++)
            {
                var mes = messages[i];
                ret[i] = new GameState.ChatMessage
                {
                    Text = mes.Message,
                    Permissions = mes.Permissions,
                    Time = mes.Time,
                    Sender = mes.UserName
                };
            }
            return ret;
        }

        public async Task<InviteResult> InviteUser(string chatroom, long inviter, long invited)
        {
            if (chatroom == null)
                throw new ArgumentException();
            if (inviter == 0)
                throw new ArgumentException();
            if (invited == 0)
                throw new ArgumentException();

            // check priviliges of the inviter and the chatroom
            var options = await _db.ChatroomOptions.Get(chatroom);
            switch (options.Invites)
            {
                case ChatroomOptions.InvitePrivilege.Members:
                    if (!await _db.ChatroomSubscribers.Exists(chatroom, inviter))
                        return InviteResult.InsufficientPermissions;

                    if (await _db.ChatroomSubscribers.Exists(chatroom, invited))
                        return InviteResult.AlreadyJoined;

                    await _db.ChatroomInvitations.Set(new ChatroomInvitation { Id = chatroom, InviterUserId = inviter, UserId = invited });
                    await _db.UserChatroomNotifications.Push(new ChatroomNotification { Id = invited, ChatroomId = chatroom, SenderUserId = inviter }, IndexSide.Left);
                    return InviteResult.Success;
                case ChatroomOptions.InvitePrivilege.Locked:
                    return InviteResult.InsufficientPermissions;
                case ChatroomOptions.InvitePrivilege.Open:
                default:
                    return InviteResult.OpenChatroom;
            }
        }

        public async Task<JoinChatroomResult> JoinChatroom(UserChatroom chatroom)
        {
            if (!chatroom.IsValid())
                throw new ArgumentException("chatroom has unassigned properties!", "chatroom");

            if (await _db.ChatroomSubscribers.Exists(chatroom.Chatroom, chatroom.UserId))
                return JoinChatroomResult.AlreadyJoined;

            var options = await _db.ChatroomOptions.Get(chatroom.Chatroom);
            switch (options.Invites)
            {
                case ChatroomOptions.InvitePrivilege.Open:
                    await _db.ChatroomSubscribers.Set(new ChatroomSubscriber { Id = chatroom.Chatroom, UserId = chatroom.UserId });
                    return JoinChatroomResult.Success;
                case ChatroomOptions.InvitePrivilege.Members:
                    if (!await _db.ChatroomInvitations.Delete(chatroom.Chatroom, chatroom.UserId))
                        return JoinChatroomResult.NotInvited;

                    await _db.ChatroomSubscribers.Set(new ChatroomSubscriber { Id = chatroom.Chatroom, UserId = chatroom.UserId });
                    await _db.UserChatroomSubscriptions.Set(new ChatroomSubscription { Id = chatroom.UserId, ChatroomId = chatroom.Chatroom });
                    return JoinChatroomResult.Success;
                case ChatroomOptions.InvitePrivilege.Locked:
                    return JoinChatroomResult.Locked;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<LeaveChatroomResult> LeaveChatroom(UserChatroom chatroom)
        {
            if (!chatroom.IsValid())
                throw new ArgumentException("chatroom has unassigned properties!", "chatroom");

            if (!await _db.ChatroomSubscribers.Delete(chatroom.Chatroom, chatroom.UserId))
                return LeaveChatroomResult.NotAMember;

            await _db.UserChatroomSubscriptions.Delete(chatroom.UserId, chatroom.Chatroom);
            return LeaveChatroomResult.Success;
        }

        //public async Task<ChatroomNotification[]> GetNotifications(long userId, long start = 0, long stop = -1)
        //{
        //    var result = await _db.UserChatroomNotifications.Range(userId, start, stop);
        //}

        public async Task<long[]> GetSubscribers(string chatroom)
        {
            var result = await _db.ChatroomSubscribers.GetAll(chatroom);
            var ret = new long[result.Length];
            for (var i = 0; i < result.Length; i++)
                ret[i] = result[i].UserId;
            return ret;
        }

        public async Task<long[]> GetInvited(string chatroom)
        {
            var result = await _db.ChatroomInvitations.GetAll(chatroom);
            var ret = new long[result.Length];
            for (var i = 0; i < result.Length; i++)
                ret[i] = result[i].UserId;
            return ret;
        }

        public async Task<ChatroomMembership> GetMembership(UserChatroom chatroom)
        {
            if (await _db.ChatroomSubscribers.Exists(chatroom.Chatroom, chatroom.UserId))
                return ChatroomMembership.Subscriber;
            if (await _db.ChatroomInvitations.Exists(chatroom.Chatroom, chatroom.UserId))
                return ChatroomMembership.Invited;
            return ChatroomMembership.NotAMember;
        }
    }

    public interface IChatroomManager
    {

    }
}
