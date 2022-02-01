using System;
using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class CanvasToWorldSpacePatches : TwoForksVrPatch
    {
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera" // Disposable camera.
        };

        private static readonly string[] canvasesToIgnore =
        {
            "com.sinai.unityexplorer_Root", // UnityExplorer.
            "com.sinai.unityexplorer.MouseInspector_Root" // UnityExplorer.
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
            try
            {
                if (IsCanvasToIgnore(__instance.name)) return;

                var canvas = __instance.GetComponent<Canvas>();

                if (!canvas) return;

                if (IsCanvasToDisable(canvas.name))
                {
                    canvas.enabled = false;
                    return;
                }

                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

                LayerHelper.SetLayer(canvas, GameLayer.UI);

                // Canvases with graphic raycasters are the ones that receive click events.
                // Those need to be handled differently, with colliders for the laser ray.
                if (canvas.GetComponent<GraphicRaycaster>())
                    AttachedUi.Create<InteractiveUi>(canvas, StageInstance.GetInteractiveUiTarget(), 0.002f);
                else
                    AttachedUi.Create<StaticUi>(canvas, StageInstance.GetStaticUiTarget(), 0.0005f);
            }
            catch (Exception exception)
            {
                Logs.LogWarning($"Failed to move canvas to world space ({__instance.name}): {exception}");
            }
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
    }
}