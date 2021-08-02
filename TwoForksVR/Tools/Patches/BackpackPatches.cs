using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Tools
{
    [HarmonyPatch(typeof(vgPlayerController), "SetBackpackVisibility")]
    public class PreventShowingBackpack
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgAttachmentController), "AttachTemporarily")]
    public class PreventAttachingBackpackToHand
    {
        public static bool Prefix(GameObject attachment)
        {
            MelonLogger.Msg($"PreventAttachingBackpackToHand {attachment.name}");
            if (attachment.name == "Backpack")
            {
                attachment.SetActive(false);
                return false;
            }
            return true;
        }
    }
}
