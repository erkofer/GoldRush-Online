using System;
using System.Collections.Generic;

namespace Caroline.Persistence.Extensions
{
    public static class EnumerableEx
    {
        public static bool TrySingle<T>(this IEnumerable<T> source, out T result)
        {
            if (source == null)
            {
                result = default(T);
                throw new ArgumentNullException("source");
            }
            var list = source as IList<T>;
            if (list != null)
            {
                switch (list.Count)
                {
                    case 0:
                        result = default(T);
                        return false;
                    case 1:
                        result = list[0];
                        return true;
                }
            }
            else
            {
                using (var e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        result = default(T);
                        return false;
                    }
                    var first = e.Current;
                    if (!e.MoveNext())
                    {
                        result = first;
                        return true;
                    }
                }
            }
            result=default(T);
            return false;
        }
    }
}
