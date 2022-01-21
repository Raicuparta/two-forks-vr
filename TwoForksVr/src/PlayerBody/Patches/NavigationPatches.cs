using System;
using System.Reflection;
using HarmonyLib;
using TwoForksVr.Helpers;
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
            // This dummy camera is used instead, so that movement direction can be independent of the camera rotation.
            var dummyCamera = new GameObject("ForwardFacingDummyCamera").AddComponent<Camera>();
            dummyCamera.transform.SetParent(__instance.transform.Find("henry"), false);
            dummyCamera.enabled = false;
            __instance.playerCamera = dummyCamera;
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerMover), nameof(vgPlayerMover.StartMoveTo))]
        public static void MovePlayerInstantly(vgPlayerMover __instance, GameObject player)
        {
            if (player == null || player.tag != "Player") return;
            
            var goalLocation = __instance.GetGoalLocation();
			goalLocation.y = player.transform.position.y;
            player.GetComponent<CharacterController>().Move(goalLocation - player.transform.position);
        }
    }
}