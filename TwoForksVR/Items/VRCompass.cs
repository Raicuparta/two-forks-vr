using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR
{
    [HarmonyPatch(typeof(vgCompass), "LateUpdate")]
    public class PatchCompass
    {
        public static bool Prefix(vgCompass __instance, Vector3 ___newRotation, float ___worldOffset)
        {
            var transform = __instance.transform;
            var forward = Vector3.ProjectOnPlane(-transform.parent.forward, Vector3.up);
            var angle = MathHelper.SignedAngle(forward, Vector3.forward, Vector3.up);
            ___newRotation.y = angle - 165f - ___worldOffset;
            transform.localEulerAngles = ___newRotation;
            return false;
        }
    }
}
