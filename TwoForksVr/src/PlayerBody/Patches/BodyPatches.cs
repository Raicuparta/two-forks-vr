using HarmonyLib;
using TwoForksVr.Locomotion;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerBody.Patches
{
    [HarmonyPatch]
    public static class BodyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.Awake))]
        public static void CreateBodyManager(vgPlayerController __instance)
        {
            VrBodyManager.Create(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.Start))]
        public static void CreateRoomScaleBodyTransform(vgPlayerController __instance)
        {
            RoomScaleBodyTransform.Create(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.SetBackpackVisibility))]
        private static bool PreventShowingBackpack()
        {
            return false;
        }
    }
}