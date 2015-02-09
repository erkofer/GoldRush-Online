using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class GameSession : IIdentifiableEntity<byte[]>
    {
        IPAddress EndPoint { get; set; }

        byte[] IIdentifiableEntity<byte[]>.Id
        {
            get { return IPAddress.Serialize(EndPoint); }
            set { EndPoint = IPAddress.Deserialize(value, 0); }
        }
    }
}
