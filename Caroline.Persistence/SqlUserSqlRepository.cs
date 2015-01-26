using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public sealed class SqlUserSqlRepository : SqlRepository<ApplicationUser, string>, IUserRepository
    {
        public SqlUserSqlRepository(GoldRushDbContext context)
            : base(context, context.Set<ApplicationUser>())
        {
        }

        public override void Remove(string id)
        {
            var entity = from e in Set
                         where e.Id == id
                         select e;
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public override async Task<ApplicationUser> Get(string id)
        {
            return await(from e in Set
                         where e.Id == id
                         select e).SingleOrDefaultAsync();
        }
    }
}