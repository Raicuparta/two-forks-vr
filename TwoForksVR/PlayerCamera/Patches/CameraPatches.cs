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
}
