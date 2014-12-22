using Caroline.Persistence.Models;

namespace Caroline.Persistence
{
    public interface IUserRepository : IRepository<ApplicationUser, string>
    {
    }
}
