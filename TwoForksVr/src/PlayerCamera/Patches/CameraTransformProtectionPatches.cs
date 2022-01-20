using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.PlayerBody;
using UnityEngine;

// Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
// We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
// All these patches try to disable any game code that would otherwise mess with the camera's transform values.
namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class CameraTransformProtectionPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.ApplyPostAnimationTransform))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateFOV))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateClipPlaneOffset))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateLookAt))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateAnimation))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateFromInput))]
        private static bool DisableCameraModifications()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
	    private static bool UpdateCameraStack(vgCameraController __instance)
	    {
		    __instance.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(__instance.transform.forward, Vector3.up), Vector3.up);
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
        private static bool DisableCameraRotation(vgCameraController __instance)
        {
            __instance.transform.position = __instance.eyeTransform.position;
            
            return false;
        }
    }
}