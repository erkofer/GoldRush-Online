namespace Caroline.Models
{
    public class LeaderboardEntry
    {
        public string UserId { get; set; }
        public long Score { get; set; }
        public long Rank { get; set; }
    }

    public class LeaderboardRequest
    {
        public long Lower { get; set; }
        public long Upper { get; set; }
    }
}