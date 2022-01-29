using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.VrCamera.Patches
{
    [HarmonyPatch]
    public class LoadingCameraPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDestroy))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDisable))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.LateUpdate))]
        private static bool SkipLoadingCameraDisableAndDestroy()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        private static void MoveLoadingCanvasToWorldSpace(vgLoadingCamera __instance)
        {
            var canvas = __instance.transform.parent.GetComponent<Canvas>();
            canvas.transform.localPosition = Vector3.zero;

            // Move loading spinner from corner to center.
            var loadSpinner = canvas.transform.Find("LoadSpinner/UI_LoadSpinner/");
            var loadSpinnerPosition = loadSpinner.localPosition;
            loadSpinner.localPosition = new Vector3(0, -150, loadSpinnerPosition.z);

            if (Object.FindObjectOfType<VrLoadingCamera>()) return;

            VrLoadingCamera.Create(__instance);
        }
    }
}