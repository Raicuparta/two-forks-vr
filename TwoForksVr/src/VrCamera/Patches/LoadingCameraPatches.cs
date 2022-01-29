using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.VrCamera.Patches
{
    [HarmonyPatch]
    public class LoadingCameraPatches
    {
        private const string VrLoadingCameraName = "VrLoadingCamera";

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

            var mainGameFlowTransform = __instance.transform.parent.parent;
            var onlyLoadOnceTransform = mainGameFlowTransform.transform.parent;
            if (onlyLoadOnceTransform.Find(VrLoadingCameraName)) return;

            var parentCamera = new GameObject(VrLoadingCameraName).AddComponent<Camera>();
            VrLoadingCamera.Create(__instance, parentCamera);
            parentCamera.cullingMask = LayerMask.GetMask("UI");
            parentCamera.transform.SetParent(onlyLoadOnceTransform, false);
            parentCamera.clearFlags = CameraClearFlags.SolidColor;
            parentCamera.backgroundColor = Color.black;

            var parentCanvas = mainGameFlowTransform.gameObject.AddComponent<Canvas>();
            parentCanvas.worldCamera = parentCamera;
            parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            var camera = __instance.GetComponent<Camera>();
            camera.enabled = false;
        }
    }
}