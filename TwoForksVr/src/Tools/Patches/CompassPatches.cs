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
        private static void SetUpVrTrackingDeviceForTrackingAngle(vgMapManager __instance, out GameObject __state)
        {
            __state = null;

            var trackingController = __instance.trackingController;

            if (!trackingController || !trackingController.player) return;

            __state = __instance.playerModel;
            __instance.playerModel = __instance.trackingController.player;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgMapManager), nameof(vgMapManager.UpdateTrackingAngle))]
        private static void ResetVrTrackingDeviceForTrackingAngle(vgMapManager __instance, GameObject __state)
        {
            if (__state == null) return;

            __instance.playerModel = __state;
        }
    }
}