using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface ILongTable
    {
        void IncrementFaf(long id, long incrementValue);
        Task<long> IncrementAsync(long id, long incrementValue = 1);
    }
}