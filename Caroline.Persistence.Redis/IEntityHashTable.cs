using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    public interface IEntityHashTable<TEntity, in TId, TField> : IDatabaseTable
    {
        Task<bool> Delete(TId id, TField member);
        Task<long> Delete(TId id, TField[] members);
        Task<bool> Exists(TId id, TField member);
        Task<TEntity> Get(TId id, TField field);
        Task<TEntity[]> Get(TId id, TField[] members);
        Task<TEntity[]> GetAll(TId id);
        Task<TField[]> Fields(TId id);
        Task<long> Length(TId id);
        Task<bool> Set(TEntity entity);
        Task Set(TId id, TEntity[] entities);
        Task<TEntity[]> Values(TId id);
    }
}
