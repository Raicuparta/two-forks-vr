using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Raicuparta.TwoForksVR
{
    class VRUIManager: MonoBehaviour
    {
        private VRUIManager()
        {

        }

        //private void Start()
        //{
        //    var hudManager = GameObject.Find("_OnlyLoadOnce").transform.Find("HUD Manager");
        //    SetUpUI(hudManager);
        //}

        private static void SetUpUI(Transform root)
        {
            root.SetParent(Camera.main.transform, false);
            root.localPosition = Vector3.forward * 0.5f;
            root.localScale = Vector3.one * 0.0004f;
            var canvases = root.GetComponentsInChildren<Canvas>(true).Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
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

        [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
        public class PatchScrimManagerShow
        {
            public static void Prefix(ref bool blur)
            {
                blur = false;
            }
        }

        //[HarmonyPatch(typeof(vgSettingsMenuController), "Start")]
        //public class PastSettingsMenuStart
        //{
        //    public static void Postfix(vgSettingsMenuController __instance)
        //    {
        //        SetUpUI(__instance.transform);
        //    }
        //}

        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        public class PatchCanvasStart
        {
            public static void Prefix(CanvasScaler __instance)
            {
                // Prevent messing with UnityExplorer
                if (__instance.name == "ExplorerCanvas")
                {
                    return;
                }
                SetUpUI(__instance.transform);
            }
        }

        [HarmonyPatch(typeof(vgSettingsManager), "minimalInterface", MethodType.Setter)]
        public class ForceMinimalInterfaceSet
        {
            public static void Prefix(ref bool value)
            {
                value = true;
            }
        }
    }
}
