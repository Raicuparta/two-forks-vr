using HarmonyLib;
using TwoForksVR.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace TwoForksVR.UI.Patches
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
            __instance.gameObject.layer = LayerFromName.UI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void CanvasScalerEnable(CanvasScaler __instance)
        {
            PatchCanvases(__instance);
        }

        private static void PatchCanvases(Component component)
        {
            var camera = Camera.main;
            if (!camera || canvasesToIgnore.Contains(component.name)) return;

            component.gameObject.layer = LayerFromName.UI;
            var canvas = component.GetComponentInParent<Canvas>();

            if (!canvas) return;

            if (canvasesToDisable.Contains(canvas.name))
            {
                canvas.enabled = false;
                return;
            }
            
            var attachToCamera = canvas.GetComponent<AttachToCamera>();
            if (attachToCamera)
            {
                
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

            canvas.worldCamera = camera;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.gameObject.layer = LayerFromName.UI;
            canvas.gameObject.AddComponent<AttachToCamera>();
            canvas.transform.localScale = Vector3.one * 0.0005f;
        }
    }
}