using UnityEngine;
using TwoForksVR.Assets;
using TwoForksVR.Stage;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;

namespace TwoForksVR
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.6")]
    public class TwoForksVRMod : BaseUnityPlugin
    {
        private static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;
            Application.logMessageReceived += OnUnityLog;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VRAssetLoader.LoadAssets();
            VRStage.Create();
        }

        private void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log)
            {
                return;
            }
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                {
                    LogError(condition);
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        LogError($"error stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
                case LogType.Warning:
                {
                    LogWarning(condition);
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        LogWarning($"warning stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
                default:
                {
                    LogInfo($"{type}: {condition}");
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        LogError($"log stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
            }
        }

        public static void LogInfo(object data)
        {
            logger.LogInfo(data);
        }

        public static void LogWarning(object data)
        {
            logger.LogWarning(data);
        }

        public static void LogError(object data)
        {
            logger.LogError(data);
        }
    }
}
