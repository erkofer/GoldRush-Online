namespace Caroline.Domain.Models
{
    public struct MessageUser
    {
        public MessageUser(string message, long userId) 
            : this()
        {
            UserId = userId;
            Message = message;
        }

        public string Message { get; private set; }
        public long UserId { get; private set; }
    }
}
