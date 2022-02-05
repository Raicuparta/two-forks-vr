using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.LaserPointer.Patches
{
    [HarmonyPatch]
    public class PlayerTargetingPatches : TwoForksVrPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerTargeting), nameof(vgPlayerTargeting.UpdateTarget))]
        private static void UserLaserForTargeting(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
        {
            var laser = StageInstance.GetLaserTransform();
            if (!StageInstance.GetLaserTransform()) return;
            cameraFacing = laser.forward;
            cameraOrigin = laser.position;
        }
    }
}