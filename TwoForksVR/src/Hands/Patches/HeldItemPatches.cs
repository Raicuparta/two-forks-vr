using System.Linq;
using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Hands.Patches
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
            if (!attachmentNameBlocklist.Contains(attachment.name)) return true;
            attachment.SetActive(false);
            return false;
        }
    }
}
