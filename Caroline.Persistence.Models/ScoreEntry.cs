using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public class ScoreEntry : IIdentifiableEntity<string>, IIdentifiableEntity <double>
    {
        public long UserId { get; set; }
        public double Score { get; set; }
        public string ListName { get; set; }

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
