using Harmony;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;
using Valve.VR;
using TwoForksVR.PlayerCamera;
using TwoForksVR.Hands;
using TwoForksVR.Assets;
using TwoForksVR.Stage;

namespace TwoForksVR
{
    public class TwoForksVRMod : MelonMod
    {
        private bool isInitialized = false;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
            VRAssetLoader.LoadAssets();

            Application.logMessageReceived += OnUnityLog;
        }

        private void OnUnityLog(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                {
                    MelonLogger.Error(condition);
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        MelonLogger.Error($"error stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
                case LogType.Warning:
                {
                    MelonLogger.Warning(condition);
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        MelonLogger.Warning($"warning stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
                default:
                {
                    MelonLogger.Msg($"{type}: {condition}");
                    if (stackTrace != null && stackTrace.Length > 0)
                    {
                        MelonLogger.Error($"log stack trace: [[ {stackTrace} ]]");
                    }
                    return;
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            if (sceneName == "Main_Menu")
            {
                SetUpMenuScene();
            } else if (sceneName.StartsWith("TeenLoop") && !isInitialized)
            {
                SetUpGameScene();
            } else if (sceneName == "Intro")
            {
                SetUpIntroScene();
            }
        }

        private void SetUpMenuScene()
        {
            isInitialized = false;
            VRStage.Create();
        }

        private void SetUpGameScene()
        {
            isInitialized = true;
            VRBodyManager.Create();
            VRStage.Create();
        }

        private void SetUpIntroScene()
        {
            VRSettings.enabled = false;
        }
    }
}
