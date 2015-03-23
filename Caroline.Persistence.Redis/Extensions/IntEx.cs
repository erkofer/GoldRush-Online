using System.Globalization;

namespace Caroline.Persistence.Redis.Extensions
{
    public static class IntEx
    {
        public static string ToStringInvariant(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
