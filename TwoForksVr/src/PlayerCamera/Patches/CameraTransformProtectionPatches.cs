using HarmonyLib;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using Valve.VR;

// Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
// We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
// These patches try to disable any game code that would otherwise mess with the camera's transform values.
namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class CameraTransformProtectionPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.ApplyPostAnimationTransform))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateFOV))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateClipPlaneOffset))]
        [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateLookAt))]
        [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateAnimation))]
        [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateFromInput))]
        private static bool DisableCameraModifications()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
        private static bool DisableCameraLateUpdate()
        {
            // If I always disable this method, it will break the camera position.
            // Since it was only broken while paused, I'm disabling it only in that scenario.
            return Time.timeScale != 0;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
        private static bool TeleportPosition(vgCameraController __instance)
        {
            var hasReachedTeleportMarked = TeleportArc.IsNextToTeleportMarker(__instance.playerController.transform);

            if (hasReachedTeleportMarked && TeleportArc.IsTeleporting())
            {
                VrStage.Instance.FadeToClear();
            }
            
            return !TeleportArc.IsTeleporting() || hasReachedTeleportMarked;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
        private static bool PreventRotationWhileTeleporting(vgCameraController __instance)
        {
            return !TeleportArc.IsTeleporting() || TeleportArc.IsNextToTeleportMarker(__instance.playerController.transform);
        }
    }
}