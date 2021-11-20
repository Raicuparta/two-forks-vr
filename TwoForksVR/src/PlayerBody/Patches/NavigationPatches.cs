using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.PlayerBody.Patches
{
    [HarmonyPatch]
    public static class NavigationPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
        public static void CreateBodyManager(vgPlayerNavigationController __instance)
        {
            // Usually NavigationController uses player camera forward as a basis for movement direction.
            // This dummy camera is used instead, so that NavigationController now only uses the player body direction.
            var dummyCamera = new GameObject("ForwardFacingDummyCamera").AddComponent<Camera>();
            // TODO dont use child index. It's because in VRBodyManager I'm using transform.parent.Rotate.
            dummyCamera.transform.SetParent(__instance.transform.GetChild(0), false);
            dummyCamera.enabled = false;
            __instance.playerCamera = dummyCamera;
        }
    }
}