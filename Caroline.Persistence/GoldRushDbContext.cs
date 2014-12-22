using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public class GoldRushDbContext : IdentityDbContext<ApplicationUser>
    {
        public GoldRushDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static GoldRushDbContext Create()
        {
            return new GoldRushDbContext();
        }
    }
}
