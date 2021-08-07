using HarmonyLib;
using UnityEngine;

namespace TwoForksVR.Tools
{
    [HarmonyPatch(typeof(vgMapManager), "UpdateMapReferences")]
    public class CreateVRMap
    {
        public static void Postfix()
        {
            // I usually wouldn't use GameObject.Find like this,
            // but this is how the base game does it, and they don't save any references.
            var mapInHand = GameObject.Find("MapInHand");
            if (mapInHand)
            {
                VRMap.Create(mapInHand.transform, "Left");
            }
        }
    }
}
