using System;
using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches;

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
        "com.sinai.unityexplorer.MouseInspector_Root", // UnityExplorer.
        "ExplorerCanvas"
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
            // This check for !Camera.main needs to stay here,
            // because without it the map texture will some times be broken. Dunno why.
            if (!Camera.main || IsCanvasToIgnore(__instance.name)) return;

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
                AttachedUi.Create<StaticUi>(canvas, StageInstance.GetStaticUiTarget(), 0.00045f);
        }
        catch (Exception exception)
        {
            Logs.WriteWarning($"Failed to move canvas to world space ({__instance.name}): {exception}");
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