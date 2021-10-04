using HarmonyLib;
using TwoForksVR.Helpers;
using UnityEngine;

namespace TwoForksVR.Tools.Patches
{
    [HarmonyPatch]
    public class CompassPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCompass), nameof(vgCompass.LateUpdate))]
        private static bool FixCompassDirection(vgCompass __instance, Vector3 ___newRotation, float ___worldOffset)
        {
            var transform = __instance.transform;
            var forward = Vector3.ProjectOnPlane(-transform.parent.forward, Vector3.up);
            var angle = MathHelper.SignedAngle(forward, Vector3.forward, Vector3.up);
            ___newRotation.y = angle - 165f - ___worldOffset;
            transform.localEulerAngles = ___newRotation;
            return false;
        }
    }
}