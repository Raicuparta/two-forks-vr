using HarmonyLib;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class GameCameraPatches
    {
        private static bool isDone;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
        private static void RecenterCamera()
        {
            if (isDone) return;
            VrStage.Instance.RecenterPosition(true);
            isDone = true;
        }

        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.Start))]
        [HarmonyPrefix]
        private static void ResetIsDoneOnCameraStart()
        {
            isDone = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        private static void ResetIsDoneOnLoading(vgLoadingCamera __instance)
        {
            isDone = false;
            __instance.GetComponent<Camera>().cullingMask = LayerMask.GetMask("UI");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraLimit), nameof(vgCameraLimit.SetLimits))]
        private static void PreventCameraVerticalRotation(ref float minVerticalAngle, ref float maxVerticalAngle)
        {
            minVerticalAngle = 0;
            maxVerticalAngle = 0;
        }
    }
}