using Harmony;
using MelonLoader;
using System;
using UnityEngine;

namespace TwoForksVR.Stage
{
    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class CreateMenuStage
    {
        [HarmonyPriority(Priority.High)]
        public static void Prefix(vgMenuCameraController __instance)
        {
            MelonLogger.Msg($"Setting up VRStage from vgMenuCameraController patch of {__instance?.name}");
            VRStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}
