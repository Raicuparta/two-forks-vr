using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR.Patches
{
    [HarmonyPatch(typeof(vgInventoryController), "CachePlayerVariables")]
    public class AttachGrabbableObjectsToVRHand
    {
        public static void Postfix(ref Transform ___pickupAttachTransform)
        {
            var hand = VRHandsManager.Instance?.RightHand;
            if (!hand)
            {
                MelonLogger.Error("Couldn't get hand transform for CachePlayerVariables patch");
                return;
            }
            ___pickupAttachTransform = hand;
        }
    }

    [HarmonyPatch(typeof(vgInventoryController), "TossStart")]
    public class SkipTossAnimation
    {
        public static bool Prefix(vgInventoryController __instance)
        {
            __instance.OnToss();
            return false;
        }
    }
}
