﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caroline.Domain;
using Caroline.Domain.Models;
using Caroline.Persistence.Models;

namespace Caroline.App
{
    public class ChatManager
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        const string PublicChatroom = "pub:pub";
        readonly ChatroomManager _rooms;

        ChatManager(ChatroomManager rooms)
        {
            _rooms = rooms;
        }

        public static async Task<ChatManager> CreateAsync()
        {
            var rooms = await ChatroomManager.CreateAsync();
            return new ChatManager(rooms);
        }

        public async Task SendPublicMessage(string message, User sender)
        {
            var permissions = GetPermissions(sender.UserName);
            switch (await _rooms.SendMessage(message, PublicChatroom, sender, permissions, false))
            {
                case SendMessageResult.Success:
                    break;
                case SendMessageResult.BadArguments:
                    break;
                case SendMessageResult.NotSubscribed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Task<GameState.ChatMessage[]> GetRecentMessages(long lastMessageRecieved = 0)
        {
            return _rooms.GetRecentMessages(PublicChatroom, lastMessageRecieved);
        }

        static readonly HashSet<string> Developers = new HashSet<string> { "Tristyn", "Hunter" };
        static readonly HashSet<string> Moderators = new HashSet<string> { "Ell dubs" };
        static readonly HashSet<string> Server = new HashSet<string> { "Server" };

        string GetPermissions(string sender)
        {
            if (Developers.Contains(sender)) return "developer";
            if (Moderators.Contains(sender)) return "moderator";
            if (Server.Contains(sender)) return "server";
            return string.Empty;
        }
    }
}
