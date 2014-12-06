using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    class GoldRushDbContext : IdentityDbContext<ApplicationUser>
    {
        public GoldRushDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
    }
}
