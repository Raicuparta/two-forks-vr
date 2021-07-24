using Harmony;
using MelonLoader;
using System;
using TwoForksVR.Stage;
using UnityEngine;

namespace TwoForksVR.PlayerCamera
{
    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class CreateMenuStage
    {
        public static void Prefix(vgMenuCameraController __instance)
        {
            MelonLogger.Msg($"Setting up VRStage from vgMenuCameraController patch of {__instance?.name}");
            VRStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}
