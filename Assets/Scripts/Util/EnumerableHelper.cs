using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Util
{
    public static class EnumerableHelper
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static IEnumerable<T> Glue<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T ac in a)
            {
                yield return ac;
            }

            foreach (T bc in b)
            {
                yield return bc;
            }
        }
    }
}