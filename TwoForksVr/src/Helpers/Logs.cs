using UnityEngine;

namespace TwoForksVr.Helpers;

public static class Logs
{
    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteInfo(object data)
    {
#if DEBUG
        Debug.Log(data);
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteWarning(object data)
    {
#if DEBUG
        Debug.LogWarning(data);
#endif
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void WriteError(object data)
    {
#if DEBUG
        Debug.LogError(data);
#endif
    }
}