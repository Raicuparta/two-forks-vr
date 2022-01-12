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
            dummyCamera.transform.SetParent(__instance.transform, false);
            dummyCamera.enabled = false;
            __instance.playerCamera = dummyCamera;

            __instance.playerRotationSpringConstant = 0;
            __instance.playerRotationDamping = 0;
            __instance.largestAllowedYawDelta = 0;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
        public static void MakeRotationInstant(vgPlayerNavigationController __instance)
        {
            // Player rotation has some acceleration which does't feel nice in VR.
            // Plus it affects some of the hacks I'm doing to rotate the player based on headset rotation.
            // This disables any acceleration and makes rotation instant.
            __instance.playerRotationSpringConstant = 0;
            __instance.playerRotationDamping = 0;
            __instance.largestAllowedYawDelta = 0;
        }
    }
}