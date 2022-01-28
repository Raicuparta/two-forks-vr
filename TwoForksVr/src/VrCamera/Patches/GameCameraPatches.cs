using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.VrCamera.Patches
{
    [HarmonyPatch]
    public class GameCameraPatches : TwoForksVrPatch
    {
        private static bool isDone;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
        private static void RecenterCamera()
        {
            if (isDone) return;
            StageInstance.RecenterPosition(true);
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgUtils), nameof(vgUtils.GetGameCamera))]
        private static bool ReplaceGetCameraResult(ref Camera __result)
        {
            var camera = StageInstance.GetMainCamera();
            if (!camera) return true;

            __result = camera;
            return false;
        }
    }
}