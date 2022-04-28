using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.UI.Patches;

[HarmonyPatch]
public static class PauseMenuPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
    private static void RemoveQuitToMenuButton(vgHudManager __instance)
    {
        var quitToMenu =
            __instance.pauseRoot.transform.Find("SafeZoner/Settings Menu Group/Quit Button");

        if (quitToMenu == null)
        {
            Logs.WriteWarning("Failed to find quit to menu button");
            return;
        }

        Object.Destroy(quitToMenu.gameObject);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgScrimManager), nameof(vgScrimManager.ShowScrim))]
    private static void DisablePauseBlur(ref bool blur)
    {
        blur = false;
    }
}