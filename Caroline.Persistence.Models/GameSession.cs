using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class GameSession : IIdentifiableEntity<string>
    {
        IpAddress EndPoint { get; set; }

        string IIdentifiableEntity<string>.Id
        {
            get { return IpAddress.Serialize(EndPoint); }
            set { EndPoint = IpAddress.Deserialize(value); }
        }
    }
}
