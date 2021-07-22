using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Stage
{

    [HarmonyPatch(typeof(vgCameraController), "Start")]
    public class CreateGameStage
    {
        public static void Prefix(vgCameraController __instance)
        {
            VRStage.Create(__instance.GetComponentInChildren<Camera>());
        }
    }

    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class CreateMenuStage
    {
        [HarmonyPriority(Priority.High)]
        public static void Prefix(vgMenuCameraController __instance)
        {
            VRStage.Create(__instance.GetComponentInChildren<Camera>());
        }
    }
}
