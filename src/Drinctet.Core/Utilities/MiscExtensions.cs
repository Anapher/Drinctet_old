using System;
using System.Collections.Generic;
using System.Linq;

namespace Drinctet.Core.Utilities
{
    public static class MiscExtensions
    {
#if NETSTANDARD
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n) =>
            source.Skip(Math.Max(0, source.Count() - n));
#endif
    }
}