using Harmony;
using MelonLoader;
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
        public static void Prefix(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
        {
            var transform = VRHandLaser.Instance?.transform;
            if (!transform)
            {
                MelonLogger.Error("Couldn't get laser transform for UpdateTarget patch");
                return;
            }
            cameraFacing = transform.forward;
            cameraOrigin = transform.position;
        }
    }
}
