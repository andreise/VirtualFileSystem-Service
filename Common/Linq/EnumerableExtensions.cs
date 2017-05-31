using System;
using System.Collections.Generic;

namespace Common.Linq
{

    public static class EnumerableExtensions
    {

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (action is null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
                action(item);
        }

        public static IEnumerable<TSource> Yield<TSource>(this TSource source)
        {
            yield return source;
        }

    }

}
