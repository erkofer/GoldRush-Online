using System.Globalization;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class DoubleEx
    {
        public static string ToStringInvariant(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
