using HarmonyLib;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class MenuCameraPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMenuCameraController), nameof(vgMenuCameraController.Start))]
        private static void CreateMenuStage(vgMenuCameraController __instance)
        {
            VrStage.Instance.SetUp(__instance.GetComponentInChildren<Camera>(), null);
        }
    }
}
