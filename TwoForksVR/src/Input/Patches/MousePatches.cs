using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.Input.Patches
{
    [HarmonyPatch]
    public static class MousePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCursorManager), nameof(vgCursorManager.Awake))]
        private static bool DestroyCursorManager(vgCursorManager __instance)
        {
            Object.Destroy(__instance);
            return false;
        }
    }
}
