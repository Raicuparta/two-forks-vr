using HarmonyLib;
using TwoForksVR.Stage;
using UnityEngine;

namespace TwoForksVR.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class MenuCameraPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMenuCameraController), nameof(vgMenuCameraController.Start))]
        private static void CreateMenuStage(vgMenuCameraController __instance)
        {
            VRStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}