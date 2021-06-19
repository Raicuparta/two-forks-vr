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
        Transform playerBody;
        private bool isVrInitialized = false;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            if (sceneName == "Main_Menu")
            {
                SetUpMenuScene();
            } else if (sceneName.StartsWith("TeenLoop") && !isVrInitialized)
            {
                SetUpGameScene();
            }
        }

        private void SetUpMenuScene()
        {
            VRSettings.enabled = false;
        }

        private void SetUpGameScene()
        {
            isVrInitialized = true;
            SetUpPlayerBody();
            new GameObject().AddComponent<VRCameraManager>();
            new GameObject().AddComponent<VRUIManager>();
            var handManager = new GameObject().AddComponent<VRHandsManager>();
            handManager.PlayerBody = playerBody;
        }

        private void SetUpPlayerBody()
        {
            playerBody = GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
            playerBody.gameObject.SetActive(false);
        }
    }
}
