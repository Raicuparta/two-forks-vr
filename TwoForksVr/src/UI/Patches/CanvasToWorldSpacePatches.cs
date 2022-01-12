using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public static class CanvasToWorldSpacePatches
    {
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera" // Disposable camera. TODO: show this information in some other way.
        };

        private static readonly string[] canvasesToIgnore =
        {
            "ExplorerCanvas" // UnityExplorer.
        };

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIBehaviour), "Awake")]
        private static void UIBehaviourAwake(UIBehaviour __instance)
        {
            LayerHelper.SetLayer(__instance, GameLayer.UI);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveCanvasesToWorldSpace(CanvasScaler __instance)
        {
            PatchCanvases(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        private static void MoveLoadingCanvasToWorldSpace(vgLoadingCamera __instance)
        {
            var canvasTransform = __instance.transform.parent;
            PatchCanvases(canvasTransform);

            // Move loading spinner from corner to center.
            var loadSpinner = canvasTransform.Find("LoadSpinner/UI_LoadSpinner/");
            var loadSpinnerPosition = loadSpinner.localPosition;
            loadSpinner.localPosition = new Vector3(0, loadSpinnerPosition.y, loadSpinnerPosition.z);
        }

        private static bool IsCanvasToIgnore(string canvasName)
        {
            foreach (var s in canvasesToIgnore)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

        private static bool IsCanvasToDisable(string canvasName)
        {
            foreach (var s in canvasesToDisable)
                if (Equals(s, canvasName))
                    return true;
            return false;
        }

        private static void PatchCanvases(Component component)
        {
            var camera = Camera.main;
            if (!camera || IsCanvasToIgnore(component.name)) return;

            var canvas = component.GetComponentInParent<Canvas>();

            if (!canvas) return;

            if (IsCanvasToDisable(canvas.name))
            {
                canvas.enabled = false;
                return;
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

            canvas.worldCamera = camera;
            canvas.renderMode = RenderMode.WorldSpace;
            LayerHelper.SetLayer(canvas, GameLayer.UI);

            // Canvases with graphic raycasters are the ones that receive click events.
            // Those need to be handled differently, with colliders for the laser ray.
            if (canvas.GetComponent<GraphicRaycaster>())
            {
                canvas.gameObject.AddComponent<InteractiveUi>();
                canvas.transform.localScale = Vector3.one * 0.002f;
            }
            else
            {
                canvas.gameObject.AddComponent<StaticUi>();
                canvas.transform.localScale = Vector3.one * 0.0005f;
            }
        }
    }
}