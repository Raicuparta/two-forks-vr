using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Hands
{
    [HarmonyPatch(typeof(vgPlayerTargeting), "UpdateTarget")]
    public class UseHandLaserForTargeting
    {
        public static Transform LaserTransform;

        public static void Prefix(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
        {
            if (!LaserTransform) return;
            cameraFacing = LaserTransform.forward;
            cameraOrigin = LaserTransform.position;
        }
    }
}
