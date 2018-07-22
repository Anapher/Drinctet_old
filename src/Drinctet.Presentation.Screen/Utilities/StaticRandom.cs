using System;
using System.Threading;

namespace Drinctet.Presentation.Screen.Utilities
{
    //https://stackoverflow.com/a/19271062
    public static class StaticRandom
    {
        static int _seed = Environment.TickCount;

        static readonly ThreadLocal<Random> Random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static int Next()
        {
            return Random.Value.Next();
        }

        public static int Next(int maxValue)
        {
            return Random.Value.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Random.Value.Next(minValue, maxValue);
        }
    }
}