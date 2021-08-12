using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVR.Assets;
using TwoForksVR.Stage;
using UnityEngine;
using UnityExplorer;

namespace TwoForksVR
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.6")]
    public class TwoForksVRMod : BaseUnityPlugin
    {
        private void Awake()
        {
            ExplorerStandalone.CreateInstance();
            Application.logMessageReceived += OnUnityLog;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VRAssetLoader.LoadAssets();
        }

        private void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Log) return;
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                {
                    LogError(condition);
                    if (!string.IsNullOrEmpty(stackTrace)) LogError($"error stack trace: [[ {stackTrace} ]]");
                    return;
                }
                case LogType.Warning:
                {
                    LogWarning(condition);
                    if (!string.IsNullOrEmpty(stackTrace))
                        LogWarning($"warning stack trace: [[ {stackTrace} ]]");
                    return;
                }
                default:
                {
                    LogInfo($"{type}: {condition}");
                    if (!string.IsNullOrEmpty(stackTrace)) LogError($"log stack trace: [[ {stackTrace} ]]");
                    return;
                }
            }
        }

        public static void LogInfo(object data)
        {
            UnityEngine.Debug.Log(data);
        }

        public static void LogWarning(object data)
        {
            UnityEngine.Debug.LogWarning(data);
        }

        public static void LogError(object data)
        {
            UnityEngine.Debug.LogError(data);
        }
    }
}