using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;
using UnityEngine.VR;

namespace TwoForksVR.Stage
{
    class VRStage: MonoBehaviour
    {
        public static VRStage Create(Camera camera)
        {
            var instance = new GameObject("VRStage").AddComponent<VRStage>();

            if (!camera)
            {
                camera = new GameObject("VR Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                return instance;
            }
            var transform = instance.transform;
            transform.SetParent(camera.transform.parent, false);
            VRCameraManager.Create(parent: transform);
            VRHandsManager.Create(
                parent: transform,
                playerBody: VRBodyManager.GetPlayerBodyTransform()
            );
            return instance;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                VRSettings.enabled = !VRSettings.enabled;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                GameObject.Find("IntroManager").SetActive(false);
                GameObject.Find("IntroTextAndBackground").SetActive(false);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }

        [HarmonyPatch(typeof(vgCameraController), "Start")]
        public class CreateGameStage
        {
            public static void Prefix(vgCameraController __instance)
            {
                Create(__instance.GetComponentInChildren<Camera>());
            }
        }

        [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
        public class CreateMenuStage
        {
            [HarmonyPriority(Priority.High)]
            public static void Prefix(vgMenuCameraController __instance)
            {
                Create(__instance.GetComponentInChildren<Camera>());
            }
        }
    }
}
