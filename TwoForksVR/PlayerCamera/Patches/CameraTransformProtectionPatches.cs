using HarmonyLib;
using UnityEngine;

// Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
// We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
// All these patches try to disable any game code that would otherwise mess with the camera's transform values.
namespace TwoForksVR.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class CameraTransformProtectionPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), "ApplyPostAnimationTransform")]
        [HarmonyPatch(typeof(vgCameraController), "UpdateFOV")]
        [HarmonyPatch(typeof(vgCameraController), "UpdateClipPlaneOffset")]
        private static bool DisableCameraModifications()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), "LateUpdate")]
        private static bool DisableCameraLateUpdate()
        {
            // If I always disable this method, it will break the camera position.
            // Since it was only broken while paused, I'm disabling it only in that scenario.
            return Time.timeScale != 0;
        }
    }
}