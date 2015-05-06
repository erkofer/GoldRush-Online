using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class ChatroomMessage : IIdentifiableEntity<string>, IIdentifiableEntity<double>
    {
        public string Id { get; set; }

        public double Index { get; set; }

        double IIdentifiableEntity<double>.Id
        {
            get { return Index; }
            set { Index = value; }
        }
    }

    public partial class ChatroomSubscriber 
        : IIdentifiableEntity<string>, IIdentifiableEntity<long>
    {
        public string Id { get; set; }

        public long UserId { get; set; }

        long IIdentifiableEntity<long>.Id
        {
            get { return UserId; }
            set { UserId = value; }
        }
    }

    public partial class ChatroomInvitation
        : IIdentifiableEntity<string>, IIdentifiableEntity<long>
    {
        public string Id { get; set; }

        public long UserId { get; set; }

        long IIdentifiableEntity<long>.Id
        {
            get { return UserId; }
            set { UserId = value; }
        }
    }

    public partial class ChatroomOptions : IIdentifiableEntity<string>
    {
        public string Id { get; set; }
    }

    public partial class ChatroomSubscription : IIdentifiableEntity<long>, IIdentifiableEntity<string>
    {
        public long Id { get; set; }

        public string ChatroomId { get; set; }

        string IIdentifiableEntity<string>.Id
        {
            get { return ChatroomId; }
            set { ChatroomId = value; }
        }
    }

    public partial class ChatroomNotification : IIdentifiableEntity<long>
    {
        public long Id { get; set; }
    }
}
