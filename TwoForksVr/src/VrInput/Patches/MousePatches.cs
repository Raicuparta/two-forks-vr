using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoForksVr.VrInput.Patches
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
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgUIInputModule), nameof(vgUIInputModule.ProcessMouseEvent))]
        private static bool DisableMouse(vgUIInputModule __instance)
        {
            return false;
        }
    }
}
