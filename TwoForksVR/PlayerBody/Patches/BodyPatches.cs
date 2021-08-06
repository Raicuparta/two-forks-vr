using Harmony;

namespace TwoForksVR.PlayerBody
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
