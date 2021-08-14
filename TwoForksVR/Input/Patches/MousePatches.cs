using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.Input.Patches
{
    [HarmonyPatch(typeof(vgCursorManager), "Awake")]
    public class PatchCursorManager
    {
        public static bool Prefix(vgCursorManager __instance)
        {
            Object.Destroy(__instance);
            return false;
        }
    }
}