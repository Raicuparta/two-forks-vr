using Harmony;
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
}
