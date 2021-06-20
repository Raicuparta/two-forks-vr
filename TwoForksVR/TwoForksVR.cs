using Harmony;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;

namespace Raicuparta.TwoForksVR
{
    public class TwoForksVR : MelonMod
    {
        private bool isInitialized = false;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
            VRAssetLoader.LoadAssets();
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
            }
        }

        private void SetUpMenuScene()
        {
            isInitialized = false;
            new GameObject().AddComponent<VRCameraManager>();
            new GameObject().AddComponent<VRUIManager>();
            new GameObject().AddComponent<VRHandsManager>();
        }

        private void SetUpGameScene()
        {
            isInitialized = true;
            new GameObject().AddComponent<VRCameraManager>();
            new GameObject().AddComponent<VRUIManager>();
            new GameObject().AddComponent<VRBodyManager>();
        }
    }
}
