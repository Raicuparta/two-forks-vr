using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class UIPatches: TwoForksVrPatch
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
    }
}