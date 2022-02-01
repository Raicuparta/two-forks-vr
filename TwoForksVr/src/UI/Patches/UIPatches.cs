using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class UIPatches : TwoForksVrPatch
    {
        private static readonly Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ShowAbilityIcon))]
        private static bool PreventShowingAbilityIcon()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.InitializeAbilityIcon))]
        private static bool DestroyAbilityIcon(vgHudManager __instance)
        {
            Object.Destroy(__instance.abilityIcon);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgScrimManager), nameof(vgScrimManager.ShowScrim))]
        private static void DisablePauseBlur(ref bool blur)
        {
            blur = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Awake))]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable))]
        private static void ChangeTMProShader(TextMeshProUGUI __instance)
        {
            try
            {
                Logs.LogInfo("ChangeTMProShader 1");
                Logs.LogInfo($"ChangeTMProShader 1 ({__instance.name})");
                if (__instance.canvas && !__instance.canvas.GetComponent<GraphicRaycaster>()) return;

                Logs.LogInfo("ChangeTMProShader 2");
                var key = __instance.font.name;

                Logs.LogInfo("ChangeTMProShader 3");
                if (!materialMap.ContainsKey(key))
                {
                    Logs.LogInfo($"ChangeTMProShader 3.5 {key}");
                    materialMap[key] = new Material(__instance.font.material)
                    {
                        shader = VrAssetLoader.TMProShader
                    };
                }

                Logs.LogInfo("ChangeTMProShader 4");
                __instance.fontMaterial = materialMap[key];
                Logs.LogInfo("ChangeTMProShader 5");
                __instance.fontBaseMaterial = materialMap[key];
                Logs.LogInfo("ChangeTMProShader 5");
                __instance.fontSharedMaterial = materialMap[key];
            }
            catch (Exception exception)
            {
                Logs.LogWarning($"Error in TMPro Patch ({__instance.name}): {exception}");
            }
        }
    }
}