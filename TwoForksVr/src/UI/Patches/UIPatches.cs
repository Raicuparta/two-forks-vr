using System;
using HarmonyLib;
using TMPro;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using Object = UnityEngine.Object;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class UIPatches : TwoForksVrPatch
    {
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
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable))]
        private static void ChangeTMProShader(TextMeshProUGUI __instance)
        {
            try
            {
                __instance.fontMaterial.shader = VrAssetLoader.TMProShader;
                __instance.fontBaseMaterial.shader = VrAssetLoader.TMProShader;
                __instance.fontSharedMaterial.shader = VrAssetLoader.TMProShader;
            }
            catch (Exception exception)
            {
                Logs.LogWarning($"Error in TMPro Patch: {exception}");
            }
        }
    }
}