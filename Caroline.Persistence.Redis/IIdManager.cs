using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IIdManager<in TEntity>
    {
        Task SetNewId(TEntity entity);
    }
}