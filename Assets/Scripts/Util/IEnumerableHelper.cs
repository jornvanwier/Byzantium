using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    // ReSharper disable once InconsistentNaming
    public static class IListHelper
    {
        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}