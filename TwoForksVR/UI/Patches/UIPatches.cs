using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.UI.Patches
{
    [HarmonyPatch]
    public static class UIPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), "ShowAbilityIcon")]
        private static bool PreventShowingAbilityIcon()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), "InitializeAbilityIcon")]
        private static bool DestroyAbilityIcon(vgHudManager __instance)
        {
            Object.Destroy(__instance.abilityIcon);
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
        private static void DisablePauseBlur(ref bool blur)
        {
            blur = false;
        }
    }
}