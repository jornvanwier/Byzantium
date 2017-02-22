using System;
using UnityEngine;

namespace Assets
{
    public static class Utils
    {
        public static void LogOperationTime(string description, Action func)
        {
            float tStart = Time.realtimeSinceStartup;
            func.Invoke();
            Debug.Log($" Performed {description} in {Time.realtimeSinceStartup - tStart} seconds");
        }

        public static T LogOperationTime<T>(string description, Func<T> func)
        {
            float tStart = Time.realtimeSinceStartup;
            T result = func.Invoke();
            Debug.Log($" Performed {description} in {Time.realtimeSinceStartup - tStart} seconds");
            return result;
        }
    }
}