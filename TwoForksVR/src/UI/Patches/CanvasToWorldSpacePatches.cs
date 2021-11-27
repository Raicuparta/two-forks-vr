using System.Linq;
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
            __instance.gameObject.layer = LayerFromName.UI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void MoveCanvasesToWorldSpace(CanvasScaler __instance)
        {
            PatchCanvases(__instance);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AkGameObj), nameof(AkGameObj.Awake))]
        private static void MoveLoadingCanvasToWorldSpace(AkGameObj __instance)
        {
            if (__instance.name != "Loading Screen") return;
            PatchCanvases(__instance);
            
            // Move loading spinner from corner to center.
            var loadSpinner = __instance.transform.Find("LoadSpinner/UI_LoadSpinner/");
            var loadSpinnerPosition = loadSpinner.localPosition;
            loadSpinner.localPosition = new Vector3(0, loadSpinnerPosition.y, loadSpinnerPosition.z);
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
                // TODO why was this here?
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
