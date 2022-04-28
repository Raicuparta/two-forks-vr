using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.Locomotion.Patches;

[HarmonyPatch]
public class NavigationPatches : TwoForksVrPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
    private static void CreateBodyManager(vgPlayerNavigationController __instance)
    {
        MovementDirection.Create(__instance, StageInstance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
    private static void MakeRotationInstant(vgPlayerNavigationController __instance)
    {
        // Player rotation has some acceleration which does't feel nice in VR.
        // Plus it affects some of the hacks I'm doing to rotate the player based on headset rotation.
        // This disables any acceleration and makes rotation instant.
        __instance.playerRotationSpringConstant = 0;
        __instance.playerRotationDamping = 0;
        __instance.largestAllowedYawDelta = 0;
    }

    // This is a workaround for a problem where Henry would some times walk off in a different direction
    // when he's supposed to walk towards an interactive object. This problem is present in the base game,
    // but it's easier to reproduce in VR.
    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgPlayerMover), nameof(vgPlayerMover.StartMoveTo))]
    private static void MovePlayerToTargetInstantly(vgPlayerMover __instance, GameObject player)
    {
        // This specific drop in a cave in late game would cause Henry to teleport beneath the floor level.
        // So we're skipping the workaround for that one.
        if (__instance.name == "HalfHeightDropEdge") return;

        if (player == null || player.tag != "Player") return;

        var goalLocation = __instance.GetGoalLocation();
        goalLocation.y = player.transform.position.y;
        player.GetComponent<CharacterController>().Move(goalLocation - player.transform.position);
    }
}