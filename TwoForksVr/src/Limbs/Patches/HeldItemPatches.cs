using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Limbs.Patches
{
    [HarmonyPatch]
    public static class HeldItemPatches
    {
        private static readonly string[] attachmentNameBlocklist =
        {
            "Backpack",
            "BackPack"
        };

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgInventoryController), nameof(vgInventoryController.TossStart))]
        private static bool SkipTossAnimation(vgInventoryController __instance)
        {
            __instance.OnToss();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgAttachmentController), nameof(vgAttachmentController.AttachTemporarily))]
        private static bool HideBlacklistedHandAttachments(GameObject attachment)
        {
            Logs.LogInfo("Attaching object to hand?");
            if (!attachment) return true;
            Logs.LogInfo($"Attaching object to hand: {attachment.name}");
            if (!IsAttachmentBlockListed(attachment.name)) return true;
            attachment.SetActive(false);
            return false;
        }

        private static bool IsAttachmentBlockListed(string name)
        {
            foreach (var s in attachmentNameBlocklist)
                if (Equals(s, name))
                    return true;
            return false;
        }
    }
}