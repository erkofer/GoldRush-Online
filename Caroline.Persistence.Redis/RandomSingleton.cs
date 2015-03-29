using System;

namespace Caroline.Persistence.Redis
{
    public static class RandomSingleton
    {
        static readonly Random Singleton = new Random();

        public static Random Instance { get { return Singleton; } }
        public static double NextDouble() { return Singleton.NextDouble(); }

        public static int Next()
        {
            return Singleton.Next();
        }

        public static int Next(int maxValue)
        {
            return Singleton.Next(maxValue);
        }

        public static int Next(int maxValue, int minValue)
        {
            return Singleton.Next(maxValue, minValue);
        }

        public static void NextBytes(byte[] buffer)
        {
            Singleton.NextBytes(buffer);
        }
    }
}
