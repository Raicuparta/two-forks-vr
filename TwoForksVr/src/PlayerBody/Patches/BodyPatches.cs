using HarmonyLib;

namespace TwoForksVr.PlayerBody.Patches
{
    [HarmonyPatch]
    public static class BodyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.SetBackpackVisibility))]
        private static bool PreventShowingBackpack()
        {
            return false;
        }
    }
}