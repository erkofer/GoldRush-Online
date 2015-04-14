using System.Globalization;
using Caroline.Persistence.Redis.Extensions;
using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class DoubleSerializer : ISerializer<double>
    {
        public byte[] Serialize(double entity)
        {
            return (RedisKey)entity.ToStringInvariant();
        }

        public double Deserialize(byte[] data)
        {
            return double.Parse((RedisKey)data, CultureInfo.InvariantCulture);
        }
    }
}