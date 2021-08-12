using HarmonyLib;

namespace TwoForksVR.Stage
{
    [HarmonyPatch]
    public static class StagePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgReset), "Awake")]
        private static void CreateStage(vgReset __instance)
        {
            VRStage.Create(__instance.transform);
        }
    }
}
