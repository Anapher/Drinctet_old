using System;
using System.Collections.Generic;

namespace Drinctet.Core.Utilities
{
    public static class TupleExtensions
    {
        public static KeyValuePair<T1, T2> ToKeyValuePair<T1, T2>(this ValueTuple<T1, T2> tuple) =>
            new KeyValuePair<T1, T2>(tuple.Item1, tuple.Item2);
    }
}