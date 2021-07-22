using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Stage
{
    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class CreateMenuStage
    {
        [HarmonyPriority(Priority.High)]
        public static void Prefix(vgMenuCameraController __instance)
        {
            VRStage.Create(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}
