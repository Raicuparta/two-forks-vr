using UnityEngine;

namespace TwoForksVr.Helpers
{
    public static class Logs
    {
        public static void LogInfo(object data)
        {
            Debug.Log(data);
        }

        public static void LogWarning(object data)
        {
            Debug.LogWarning(data);
        }

        public static void LogError(object data)
        {
            Debug.LogError(data);
        }
    }
}
