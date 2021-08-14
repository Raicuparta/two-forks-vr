using HarmonyLib;

namespace TwoForksVR.PlayerBody.Patches
{
    [HarmonyPatch(typeof(vgPlayerController), "SetBackpackVisibility")]
    public class PreventShowingBackpack
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}