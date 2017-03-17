using System;
using UnityEngine;

namespace Assets.Util
{
    public static class Utils
    {
        public static void LogOperationTime(string description, Action func)
        {
            DateTime now = DateTime.Now;
            func.Invoke();
            Debug.Log($"Performed {description} in {(DateTime.Now - now).TotalMilliseconds} milliseconds");
        }

        public static T LogOperationTime<T>(string description, Func<T> func)
        {
            DateTime now = DateTime.Now;
            T result = func.Invoke();
            Debug.Log($"Performed {description} in {(DateTime.Now - now).TotalMilliseconds} milliseconds");
            return result;
        }

        public static Vector3 DeltaPointBetween(Vector3 a, Vector3 b, float percentage = 0.5f)
        {
            Vector3 delta = b - a;
            delta *= percentage;
            return delta;
        }
    }
}