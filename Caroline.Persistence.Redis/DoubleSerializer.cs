using System.Globalization;
using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Redis
{
    public class DoubleSerializer : ISerializer<double>
    {
        public byte[] Serialize(double entity)
        {
            return entity.ToStringInvariant().GetBytesNoEncoding();
        }

        public double Deserialize(byte[] data)
        {
            return double.Parse(data.GetStringNoEncoding(), CultureInfo.InvariantCulture);
        }
    }
}