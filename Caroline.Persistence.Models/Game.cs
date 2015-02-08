using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class Game : IIdentifiableEntity<long>
    {
        public long Id { get; set; }
    }
}
