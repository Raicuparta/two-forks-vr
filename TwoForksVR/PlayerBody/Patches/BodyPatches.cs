using HarmonyLib;

namespace TwoForksVR.PlayerBody.Patches
{
    [HarmonyPatch]
    public class BodyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.SetBackpackVisibility))]
        private static bool PreventShowingBackpack()
        {
            return false;
        }
    }
}