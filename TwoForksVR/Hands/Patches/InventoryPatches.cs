using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Hands
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
        private static readonly string[] AttachmentNameBlacklist = new string[]
        {
            "Backpack",
            "BackPack"
        };

        public static bool Prefix(GameObject attachment)
        {
            MelonLogger.Msg($"Attaching object to hand: {attachment.name}");
            if (AttachmentNameBlacklist.Contains(attachment.name))
            {
                attachment.SetActive(false);
                return false;
            }
            return true;
        }
    }
}
