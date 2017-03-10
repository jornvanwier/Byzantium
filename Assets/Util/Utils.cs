using System;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Assets.Util
{
    public static class Utils
    {
        public static void LogOperationTime(string description, Action func)
        {
            float tStart = Time.realtimeSinceStartup;
            func.Invoke();
            Debug.Log($"Performed {description} in {Time.realtimeSinceStartup - tStart} seconds");
        }

        public static T LogOperationTime<T>(string description, Func<T> func)
        {
            float tStart = Time.realtimeSinceStartup;
            T result = func.Invoke();
            Debug.Log($"Performed {description} in {Time.realtimeSinceStartup - tStart} seconds");
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