using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static void Prefix(CanvasScaler __instance)
        {
            // Prevent messing with UnityExplorer
            if (__instance.name == "ExplorerCanvas")
            {
                return;
            }

            var transform = __instance.transform;

            transform.SetParent(Camera.main.transform, false);
            transform.localPosition = Vector3.forward * 0.5f;
            transform.localScale = Vector3.one * 0.0004f;
            var canvases = transform.GetComponentsInChildren<Canvas>(true).Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
            canvases.Do(canvas =>
            {
                if (canvas.name == "BlackBars")
                {
                    canvas.enabled = false;
                    return;
                }
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.localPosition = Vector3.zero;
                canvas.transform.localRotation = Quaternion.identity;
                canvas.transform.localScale = Vector3.one;
            });
        }
    }
}
