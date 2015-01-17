using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public sealed class UserRepository : Repository<ApplicationUser, string>, IUserRepository
    {
        public UserRepository(GoldRushDbContext context)
            : base(context, context.Set<ApplicationUser>())
        {
        }

        public override void Remove(string id)
        {
            var entity = from e in Set
                         where e.Id == id
                         select e;
            EfContext.Entry(entity).State = EntityState.Deleted;
        }

        public override async Task<ApplicationUser> Get(string id)
        {
            return await(from e in Set
                         where e.Id == id
                         select e).SingleOrDefaultAsync();
        }
    }
}