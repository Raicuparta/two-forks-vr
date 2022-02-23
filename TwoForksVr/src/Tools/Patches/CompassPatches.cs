using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
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
        [HarmonyPatch(typeof(vgCompass), nameof(vgCompass.Start))]
        private static void AddHandednessMirrorToCompass(vgCompass __instance)
        {
            __instance.gameObject.AddComponent<VrHandednessXMirror>();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgTrackingDeviceController), nameof(vgTrackingDeviceController.Start))]
        private static void CreateVrTrackingDevice(vgTrackingDeviceController __instance)
        {
            // vgTrackingDeviceController.player is used for calculating the compass angle.
            // This doesn't work in VR, since the hands can move and rotate independently of the player body.
            // So we replace it for a VrTrackingDevice gameobject, which points in the correct direction.
            __instance.player = VrTrackingDevice.Create(__instance).gameObject;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMapManager), nameof(vgMapManager.UpdateTrackingAngle))]
        private static void SetUpVrTrackingDeviceForTrackingAngle(vgMapManager __instance, out GameObject __state)
        {
            __state = null;

            var trackingController = __instance.trackingController;

            if (!trackingController || !trackingController.player) return;

            // Store the original player model reference in the patch state.
            __state = __instance.playerModel;

            // Temporarily replace the player model reference with the VrTrackingDevice,
            // stored in vgTrackingDeviceController.player in a previous patch.
            __instance.playerModel = __instance.trackingController.player;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgMapManager), nameof(vgMapManager.UpdateTrackingAngle))]
        private static void ResetVrTrackingDeviceForTrackingAngle(vgMapManager __instance, GameObject __state)
        {
            if (__state == null) return;

            // Reset vgMapManager.playerModel with the original value, stored in the prefix patch state.
            __instance.playerModel = __state;
        }
    }
}