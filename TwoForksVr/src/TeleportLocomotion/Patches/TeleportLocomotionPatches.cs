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
            
            var hasReachedTeleportMarked = TeleportArc.IsNextToTeleportMarker(__instance.playerController.transform);

            if (hasReachedTeleportMarked && TeleportArc.IsTeleporting())
            {
                VrStage.Instance.FadeToClear();
            }
            
            return !TeleportArc.IsTeleporting() || hasReachedTeleportMarked;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
        private static bool PreventRotationWhileTeleporting(vgCameraController __instance)
        {
            if (!__instance.playerController.navController.enabled)
            {
                return false;
            }

            return !TeleportArc.IsTeleporting() || TeleportArc.IsNextToTeleportMarker(__instance.playerController.transform);
        }
    }
}