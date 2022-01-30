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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgTrackingDeviceController), nameof(vgTrackingDeviceController.Start))]
        private static void CreateVrTrackingDevice(vgTrackingDeviceController __instance)
        {
            VrTrackingDevice.Create(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMapManager), nameof(vgMapManager.UpdateTrackingAngle))]
        private static bool FixTrackingAngle(vgMapManager __instance)
        {
            if (__instance.trackingController == null) return true;

            var trackingTarget = __instance.GetTrackingTarget();
            if (trackingTarget == null) return false;

            var vector =
                __instance.trackingController.player.transform.InverseTransformPoint(trackingTarget.transform.position);
            __instance.angleToTarget = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
            if (__instance.trackingController != null)
                __instance.trackingController.SetAngle(__instance.angleToTarget);

            return false;
        }
    }
}