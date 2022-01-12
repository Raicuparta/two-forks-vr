using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.LaserPointer.Patches
{
    [HarmonyPatch]
    public static class PlayerTargetingPatches
    {
        public static Transform LaserTransform;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerTargeting), nameof(vgPlayerTargeting.UpdateTarget))]
        private static void UserLaserForTargeting(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
        {
            if (!LaserTransform) return;
            cameraFacing = LaserTransform.forward;
            cameraOrigin = LaserTransform.position;
        }
    }
}
