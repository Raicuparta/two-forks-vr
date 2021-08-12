using HarmonyLib;
using TwoForksVR.Stage;
using UnityEngine;

namespace TwoForksVR.PlayerCamera.Patches
{
    [HarmonyPatch(typeof(vgMenuCameraController), "Start")]
    public class CreateMenuStage
    {
        public static void Prefix(vgMenuCameraController __instance)
        {
            VRStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}