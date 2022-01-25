using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class MousePatches: TwoForksVrPatch
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
        [HarmonyPatch(typeof(vgUIInputModule), nameof(vgUIInputModule.GetDefaultSelectedGameObject))]
        private static bool DisableMouse(vgUIInputModule __instance)
        {
            return false;
        }
    }
}