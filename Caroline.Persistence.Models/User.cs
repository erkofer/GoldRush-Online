using Caroline.Persistence.Redis;
using Microsoft.AspNet.Identity;

namespace Caroline.Persistence.Models
{
    public partial class User : IUser<long>, IIdentifiableEntity<long>
    {
        // User identity generation?
	    public long Id { get; set; }
    }
}
