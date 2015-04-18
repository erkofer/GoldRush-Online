namespace Caroline.Domain.Models
{
    public class Message
    {
        public string Chatroom { get; set; }
        public string Body { get; set; }
        public long UserId { get; set; }

        public bool IsValid()
        {
            return Chatroom != null && Body != null && UserId != 0;
        }
    }
}
