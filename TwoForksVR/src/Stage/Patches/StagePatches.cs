using HarmonyLib;

namespace TwoForksVR.Stage.Patches
{
    [HarmonyPatch]
    public static class StagePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgReset), nameof(vgReset.Awake))]
        private static void CreateStage(vgReset __instance)
        {
            // All objects are eventually destroyed, unless they are children of this "reset object".
            // So we make the reset object the parent of the VR Stage, to make sure we keep it alive.
            VRStage.Create(__instance.transform);
        }
    }
}