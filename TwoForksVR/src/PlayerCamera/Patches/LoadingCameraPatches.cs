using HarmonyLib;

namespace TwoForksVr.PlayerCamera.Patches
{    
    [HarmonyPatch]
    public class LoadingCameraPatches
    {
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDestroy))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDisable))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.LateUpdate))]
        private static bool SkipLoadingCameraDisableAndDestroy()
        {
            return false;
        }
    }
}