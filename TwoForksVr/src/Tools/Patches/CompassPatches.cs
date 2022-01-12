using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Tools.Patches
{
    [HarmonyPatch]
    public class CompassPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCompass), nameof(vgCompass.LateUpdate))]
        private static bool FixCompassDirection(vgCompass __instance)
        {
            var transform = __instance.transform;
            var forward = Vector3.ProjectOnPlane(-transform.parent.forward, Vector3.up);
            var angle = MathHelper.SignedAngle(forward, Vector3.forward, Vector3.up);
            __instance.newRotation.y = angle - 165f - __instance.worldOffset;
            transform.localEulerAngles = __instance.newRotation;
            return false;
        }
    }
}