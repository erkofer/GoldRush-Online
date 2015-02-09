using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public interface IDatabaseArea
    {
        IDatabaseArea CreateSubArea(RedisKey area);
        IDatabase Area { get; }
        CarolineScriptsRepo Scripts { get; }
    }
}