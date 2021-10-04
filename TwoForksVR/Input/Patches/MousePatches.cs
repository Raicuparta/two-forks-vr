using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.Input.Patches
{
    [HarmonyPatch]
    public static class MousePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCursorManager), nameof(vgCursorManager.Awake))]
        private static bool PatchCursorManager(vgCursorManager __instance)
        {
            Object.Destroy(__instance);
            return false;
        }
    }
}