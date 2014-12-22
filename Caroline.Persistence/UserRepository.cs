using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public sealed class UserRepository : Repository<ApplicationUser, string>, IUserRepository
    {
        public UserRepository(GoldRushDbContext context)
            : base(context, context.Set<ApplicationUser>())
        {
        }
    }
}