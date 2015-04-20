using System;
using System.Threading.Tasks;
using Caroline.Domain.Models;
using Caroline.Persistence;
using Caroline.Persistence.Models;
using Caroline.Persistence.Redis;

namespace Caroline.Domain
{
    class ChatroomManager : IChatroomManager
    {
        const long ChatroomMessageCapacity = 25;
        const int MaxMessageLength = 200;
        readonly CarolineRedisDb _db;

        ChatroomManager(CarolineRedisDb db)
        {
            _db = db;
        }

        public static async Task<ChatroomManager> Create()
        {
            return new ChatroomManager(await CarolineRedisDb.CreateAsync());
        }

        public async Task<SendMessageResult> SendMessage(Message message)
        {
            if (!message.IsValid())
                return SendMessageResult.BadArguments;

            if (await _db.ChatroomSubscribers.Get(message.Chatroom, message.UserId) == null)
                return SendMessageResult.NotSubscribed;

            var body = message.Body;
            if (body.Length > MaxMessageLength)
                body = body.Substring(0, MaxMessageLength);
            // todo: chat swear filtering and shitty fuck tits

            // update message list in chatroom
            var numMessages = await _db.ChatroomMessages.Push(new ChatroomMessage { Id = message.Chatroom, Message = body, UserId = message.UserId }, IndexSide.Left);
            if (numMessages > ChatroomMessageCapacity * 2)
            {
                var numToRemove = numMessages - ChatroomMessageCapacity;
                await _db.ChatroomMessages.Pop(message.Chatroom, IndexSide.Right, numToRemove);
                // TODO: move old messages from the result of the pop operation to a persistent nosql store
            }

            // message fanout
            var subscribers = await _db.ChatroomSubscribers.GetAll(message.Chatroom);
            for (var i = 0; i < subscribers.Length; i++)
            {
                var subscriber = subscribers[i];
                var numNotifications = await _db.UserChatroomNotifications.Push(new ChatroomNotification
                {
                    Id = subscriber.UserId,
                    ChatroomId = message.Chatroom,
                    Message = body,
                    SenderUserId = message.UserId
                }, IndexSide.Left);

                if (numNotifications < ChatroomMessageCapacity * 2)
                    continue;
                // we dont store older notifications, as we can generate it by looking at what chatrooms the user is subscribed to
                var numToRemove = numMessages - ChatroomMessageCapacity;
                await _db.UserChatroomNotifications.Pop(numNotifications - ChatroomMessageCapacity, IndexSide.Right, numToRemove);
            }

            return SendMessageResult.Success;
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

                    await _db.ChatroomInvitations.Set(new ChatroomInvitation{Id = chatroom, InviterUserId = inviter, UserId = invited});
                    await _db.UserChatroomNotifications.Push(new ChatroomNotification{ Id = invited, ChatroomId = chatroom, SenderUserId = inviter}, IndexSide.Left);
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
