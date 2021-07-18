using Harmony;
using UnityEngine;

namespace TwoForksVR.Hands
{
    [HarmonyPatch(typeof(vgExtensionMethods), "SearchByName")]
    public class ForceFindVRHandAttachment
    {
        public static bool Prefix(ref Transform __result, string name)
        {
            if (name == "henryHandLeftAttachment")
            {
                __result = VRHandsManager.Instance.LeftHandAttachment;
                return false;
            }
            else if (name == "henryHandRightAttachment")
            {
                __result = VRHandsManager.Instance.RightHandAttachment;
                return false;
            }
            return true;
        }
    }
}
