using Caroline.Persistence.Models;

namespace Caroline.Domain.Models
{
    public class UserProfile
    {
        public long Id { get; set; }
        public bool IsAnonymous { get; set; }
        public string UserName { get; set; }
        public ScoreEntry ScoreboardRank { get; set; }
    }
}
