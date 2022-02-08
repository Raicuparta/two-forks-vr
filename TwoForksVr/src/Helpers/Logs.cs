using System;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    public static class Logs
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static void LogInfo(object data)
        {
#if DEBUG
            Debug.Log(data);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void LogWarning(object data)
        {
#if DEBUG
            Debug.LogWarning(data);
#endif
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void LogError(object data)
        {
#if DEBUG
            Debug.LogError($"{data}: {Environment.StackTrace}");
#endif
        }
    }
}