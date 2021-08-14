using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.Hands.Patches
{
    [HarmonyPatch(typeof(vgInventoryController), "TossStart")]
    public class SkipTossAnimation
    {
        public static bool Prefix(vgInventoryController __instance)
        {
            __instance.OnToss();
            return false;
        }
    }


    [HarmonyPatch(typeof(vgAttachmentController), "AttachTemporarily")]
    public class HideBlacklistedHandAttachments
    {
        private static readonly string[] attachmentNameBlacklist =
        {
            "Backpack",
            "BackPack"
        };

        public static bool Prefix(GameObject attachment)
        {
            TwoForksVRMod.LogInfo("Attaching object to hand?");
            if (!attachment) return true;
            TwoForksVRMod.LogInfo($"Attaching object to hand: {attachment.name}");
            if (!attachmentNameBlacklist.Contains(attachment.name)) return true;
            attachment.SetActive(false);
            return false;

        }
    }
}