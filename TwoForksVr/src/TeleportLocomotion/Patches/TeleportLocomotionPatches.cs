using HarmonyLib;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.UI;

namespace TwoForksVr.TeleportLocomotion.Patches
{
    [HarmonyPatch]
    public static class TeleportLocomotionPatches
    {
                [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
        private static bool TeleportPosition(vgCameraController __instance)
        {
            if (VrSettings.FixedCameraDuringAnimations.Value && !__instance.playerController.navController.enabled)
            {
                return false;
            }
            
            var hasReachedTeleportMarked = TeleportController.IsNextToTeleportMarker(__instance.playerController.transform);

            if (hasReachedTeleportMarked && TeleportController.IsTeleporting())
            {
                VrStage.Instance.FadeToClear();
            }
            
            return !TeleportController.IsTeleporting() || hasReachedTeleportMarked;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
        private static bool PreventRotationWhileTeleporting(vgCameraController __instance)
        {
            if (!__instance.playerController.navController.enabled)
            {
                return false;
            }

            return !TeleportController.IsTeleporting() || TeleportController.IsNextToTeleportMarker(__instance.playerController.transform);
        }
    }
}