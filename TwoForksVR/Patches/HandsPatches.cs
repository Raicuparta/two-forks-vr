using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Patches
{
    [HarmonyPatch(typeof(vgInventoryController), "CachePlayerVariables")]
    public class PatchCachePlayerVariables
    {
        public static void Postfix(ref Transform ___pickupAttachTransform)
        {
            var hand = VRHandsManager.Instance?.RightHand;
            if (!hand)
            {
                return;
            }
            ___pickupAttachTransform = hand;
        }
    }

    [HarmonyPatch(typeof(vgInventoryController), "TossStart")]
    public class PatchTossAnimation
    {
        public static bool Prefix(vgInventoryController __instance)
        {
            __instance.OnToss();
            return false;
        }
    }
}
