using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class SaveState : IIdentifiableEntity<long>
    {
        public long Id { get; set; }
    }
}
