using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
// We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
// All these patches try to disable any game code that would otherwise mess with the camera's transform values.
namespace TwoForksVR.PlayerCamera
{
    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class DisableMenuCameraMovement
    {
        public static bool Prefix(vgMenuCameraController __instance)
        {
            UnityEngine.Object.Destroy(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "ApplyPostAnimationTransform")]
    public class DisableGameCameraMovement
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "UpdateFOV")]
    public class DisableCameraUpdateFOV
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "UpdateClipPlaneOffset")]
    public class DisableCameraClipPlaneOffset
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LateUpdate")]
    public class DisableCameraLateUpdate
    {
        public static bool Prefix()
        {
            // If I always disable this method, it will break the camera position.
            // Since it was only broken while paused, I'm disabling it only in that scenario.
            if (Time.timeScale == 0)
            {
                return false;
            }
            return true;
        }
    }
}
