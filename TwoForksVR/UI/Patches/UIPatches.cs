using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVR.UI
{
    [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
    public class DisablePauseBlur
    {
        public static void Prefix(ref bool blur)
        {
            blur = false;
        }
    }

    [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
    public class MoveCanvasToWorldSpace
    {
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera", // Disposable camera. TODO: show this information in some other way.
        };
        private static readonly string[] canvasesToIgnore =
{
            "ExplorerCanvas", // UnityExplorer.
        };

        public static void Prefix(CanvasScaler __instance)
        {
            if (!Camera.main || canvasesToIgnore.Contains(__instance.name))
            {
                return;
            }

            var canvas = __instance.GetComponent<Canvas>();

            if (!canvas)
            {
                MelonLogger.Error($"MoveCanvasToWorldSpace: {__instance.name} has no Canvas");
                return;
            }

            if (canvasesToDisable.Contains(canvas.name))
            {
                canvas.enabled = false;
                return;
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return;
            }

            canvas.worldCamera = Camera.main;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.gameObject.AddComponent<AttachToCamera>();
            __instance.transform.localScale = Vector3.one * 0.0004f;
        }
    }
}
