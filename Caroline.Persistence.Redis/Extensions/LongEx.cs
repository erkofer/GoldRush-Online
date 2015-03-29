using System.Globalization;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class LongEx
    {
        public static string ToStringInvariant(this long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
