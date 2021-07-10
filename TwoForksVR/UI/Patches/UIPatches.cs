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
            "Camera", // Disposable camera.
        };
        private static readonly string[] canvasesToIgnore =
{
            "ExplorerCanvas", // UnityExplorer.
        };

        public static void Prefix(CanvasScaler __instance)
        {
            if (canvasesToIgnore.Contains(__instance.name))
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
                if (canvasesToDisable.Contains(canvas.name))
                {
                    canvas.enabled = false;
                    return;
                }
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.localPosition = Vector3.zero;
                canvas.transform.localRotation = Quaternion.identity;
                canvas.transform.localScale = Vector3.one;

                if (canvas.name == "Camera")
                {
                    canvas.transform.SetParent(VRHandsManager.Instance.RightHand);
                    canvas.transform.localScale = Vector3.one * 0.3f;
                }
            });
        }
    }
}
