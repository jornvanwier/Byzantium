using System.Collections.Generic;

namespace Assets.Scripts.Util
{
    public static class EnumeratorHelper
    {
        public static IEnumerable<T> Iterate<T>(this IEnumerator<T> iterator)
        {
            while (iterator.MoveNext())
                yield return iterator.Current;
        }
    }
}