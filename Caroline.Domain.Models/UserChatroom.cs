namespace Caroline.Domain.Models
{
    public class UserChatroom
    {
        public long UserId { get; set; }
        public string Chatroom { get; set; }

        public bool IsValid()
        {
            return UserId != 0 && Chatroom != null;
        }
    }
}
