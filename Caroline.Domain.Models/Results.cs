namespace Caroline.Domain.Models
{
    public enum SendMessageResult
    {
        Success,
        BadArguments,
        NotSubscribed
    }

    public enum JoinChatroomResult
    {
        Success,
        BadArguments,
        AlreadyJoined,
        NotInvited,
        Locked
    }

    public enum LeaveChatroomResult
    {
        Success,
        NotAMember
    }

    public enum ChatroomMembership
    {
        Subscriber,
        Invited,
        NotAMember
    }

    public enum InviteResult
    {
        Success,
        InsufficientPermissions,
        OpenChatroom,
        AlreadyJoined
    }
}
