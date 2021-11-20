using HarmonyLib;

namespace TwoForksVr.PlayerBody.Patches
{
    [HarmonyPatch]
    public static class BodyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.Awake))]
        public static void CreateBodyManager(vgPlayerController __instance)
        {
            VRBodyManager.Create(__instance);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.SetBackpackVisibility))]
        private static bool PreventShowingBackpack()
        {
            return false;
        }
    }
}
