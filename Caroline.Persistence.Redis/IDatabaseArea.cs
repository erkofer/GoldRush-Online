using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public interface IDatabaseArea : IDatabase
    {
        IDatabaseArea CreateSubArea(RedisKey area);
        IDatabase Parent { get; }
        CarolineScriptsRepo Scripts { get; }
    }
}