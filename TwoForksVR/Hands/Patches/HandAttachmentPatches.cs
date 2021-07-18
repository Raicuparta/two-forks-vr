using Harmony;
using UnityEngine;

namespace TwoForksVR.Hands
{
    [HarmonyPatch(typeof(vgExtensionMethods), "SearchByName")]
    public class ForceFindVRHandAttachment
    {
        public static bool Prefix(ref Transform __result, string name)
        {
            Transform attachment = null;
            if (name == "henryHandLeftAttachment")
            {
                attachment = VRHandsManager.Instance?.LeftHandAttachment;
            }
            else if (name == "henryHandRightAttachment")
            {
                attachment = VRHandsManager.Instance?.RightHandAttachment;
            }

            if (attachment)
            {
                __result = attachment;
                return false;
            }
            return true;
        }
    }
}
