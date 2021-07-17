using Harmony;
using MelonLoader;
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
        // VR Camera's position always relative to the player's feet (when playing in room scale).
        // Pancake Camera's position was relative to this "eyeTransform", which is in the player's head.
        // If we just say that the "eyeTransform" is actually located at the player's feet, then the VR camera
        // will be correctly placed relative to the floor. Just needs to be adjusted to allow for sitting mode.
        public static void Postfix(ref Transform ___eyeTransform, GameObject ___playerGameObject)
        {
            //if (!VRCameraManager.Instance)
            //{
            //    MelonLogger.Msg("whaaat?");
            //}
            //VRCameraManager.Instance.MoveCameraToCorrectHeight();
        }
    }
}
