﻿using Harmony;

namespace TwoForksVR.Input
{
    [HarmonyPatch(typeof(vgCursorManager), "Awake")]
    public class PatchCursorManager
    {
        public static bool Prefix(vgCursorManager __instance)
        {
            UnityEngine.Object.Destroy(__instance);
            return false;
        }
    }
}