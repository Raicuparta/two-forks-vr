using HarmonyLib;
using TwoForksVr.Settings;

namespace TwoForksVr.Locomotion.Patches;

[HarmonyPatch]
public class TeleportLocomotionPatches : TwoForksVrPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
    private static bool TeleportPosition(vgCameraController __instance)
    {
        if (VrSettings.FixedCameraDuringAnimations.Value &&
            !__instance.playerController.navController.enabled) return false;

        return !StageInstance.IsTeleporting() ||
               StageInstance.IsNextToTeleportMarker(__instance.playerController.transform);
    }
}