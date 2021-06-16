using Harmony;
using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;

namespace Raicuparta.UnityVRCameraReparent
{
    public class UnityVRCameraReparent : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            var cameraController = GameObject.FindObjectOfType<vgCameraController>();
            if (!cameraController)
            {
                return;
            }

            cameraController.defaultCameraTuning.ForEach(tuning => {
                tuning.minVerticalAngle = 0;
                tuning.maxVerticalAngle = 0;
            });
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                ReparentCamera();
            }
        }

        private void ReparentCamera()
        {
            MelonLogger.Msg("Reparenting camera");

            var mainCamera = Camera.main.transform;
            var vrCameraParent = new GameObject().transform;
            vrCameraParent.SetParent(mainCamera.parent, false);
            mainCamera.SetParent(vrCameraParent);
            vrCameraParent.localPosition = Vector3.down;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanUp")]
    public class PatchLeanUp
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanDown")]
    public class PatchLeanDown
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanVertical")]
    public class PatchGetRoll
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            MelonLogger.Msg("Prefix");
            return false;
        }
    }
}
