using System.Data.Entity;
using System.Threading.Tasks;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public class GoldRushDbContext : IdentityDbContext<ApplicationUser>, IGoldRushDbContext
    {
        public GoldRushDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static GoldRushDbContext Create()
        {
            return new GoldRushDbContext();
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }

        public DbContext GetContext()
        {
            // some nastiness because DbContext doesn't have a base 
            // interface that we can inherit from
            return this;
        }

        public new Task SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
