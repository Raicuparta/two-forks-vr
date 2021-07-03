using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public class MousePatches : MonoBehaviour
    {
        [HarmonyPatch(typeof(vgCursorManager), "Awake")]
        public class PatchCursorManager
        {
            public static bool Prefix(vgCursorManager __instance)
            {
                Destroy(__instance);
                return false;
            }
        }
    }
}
