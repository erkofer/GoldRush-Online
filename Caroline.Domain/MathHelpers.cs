namespace Caroline.Domain
{
    static class MathHelpers
    {
        public static long Difference(long a, long b)
        {
            if (a > b)
                return a - b;
            return b - a;
        }
    }
}
