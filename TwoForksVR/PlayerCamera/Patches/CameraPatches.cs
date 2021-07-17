using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.PlayerCamera
{
    [HarmonyPatch(typeof(vgCameraController), "Start")]
    public class CameraPatches
    {
        public static void Postfix(ref Transform ___eyeTransform, GameObject ___playerGameObject)
        {
            ___eyeTransform = ___playerGameObject.transform;
        }
    }

    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class DisableMenuCameraMovement
    {
        // Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
        // We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
        public static bool Prefix(vgMenuCameraController __instance)
        {
            UnityEngine.Object.Destroy(__instance);
            return false;
        }
    }
}
