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
}
