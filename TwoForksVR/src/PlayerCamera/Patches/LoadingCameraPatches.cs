using HarmonyLib;

namespace TwoForksVr.PlayerCamera.Patches
{    
    [HarmonyPatch]
    public class LoadingCameraPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        private static bool DisableVgLoadingCamera(vgLoadingCamera __instance)
        {
            __instance.enabled = false;
            return false;
        }
        
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDestroy))]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDisable))]
        private static bool SkipLoadingCameraDisableAndDestroy()
        {
            return false;
        }
    }
}