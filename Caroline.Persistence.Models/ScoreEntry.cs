using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public class ScoreEntry : IIdentifiableEntity<long>, IIdentifiableEntity<string>, IIdentifiableEntity <double>
    {
        public long UserId { get; set; }
        public double Score { get; set; }
        public string ListName { get; set; }

        long IIdentifiableEntity<long>.Id
        {
            get { return UserId; }
            set { UserId = value; }
        }

        public double Id
        {
            get { return Score; }
            set { Score = value; }
        }

        string IIdentifiableEntity<string>.Id
        {
            get { return ListName; }
            set { ListName = value; }
        }
    }
}
