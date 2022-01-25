using HarmonyLib;
using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Locomotion.Patches
{
    [HarmonyPatch]
    public class TeleportLocomotionPatches : TwoForksVrPatch
    {
        public static TeleportController Teleport;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
        private static bool TeleportPosition(vgCameraController __instance)
        {
            if (VrSettings.FixedCameraDuringAnimations.Value &&
                !__instance.playerController.navController.enabled) return false;

            if (!Teleport) return true;

            return !Teleport.IsTeleporting() || Teleport.IsNextToTeleportMarker(__instance.playerController.transform);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
        private static void TriggerTeleportBlinkAnimation(vgCameraController __instance)
        {
            if (!Teleport) return;

            var hasReachedTeleportMarked = Teleport.IsNextToTeleportMarker(__instance.playerController.transform);

            if (hasReachedTeleportMarked && Teleport.IsTeleporting()) StageInstance.FadeToClear();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.ForwardMovement))]
        private static void OverrideForwardMovementWithTeleport(vgPlayerController __instance, float axisValue)
        {
            if (!VrSettings.Teleport.Value || !__instance.navController.enabled) return;

            __instance.forwardInput = Mathf.Max(0, Teleport.IsTeleporting() ? 1 : 0);
        }
    }
}