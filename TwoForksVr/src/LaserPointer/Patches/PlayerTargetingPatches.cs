using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.LaserPointer.Patches;

[HarmonyPatch]
public class PlayerTargetingPatches : TwoForksVrPatch
{
    // Distance from targeting origin and the targetable object is used for deciding which interactions to allow.
    // Some times, this makes it hard to target some objects with our hands if we're standing too close to them.
    // This is because the original targeting origin was in the player's head, and now it's in the hand laser, which
    // is naturally positioned a bit more forward than the head. 
    // I tried a bunch of nice ways to fix this, but the only reliable solution was this hack, where I make the
    // targeting ray start at an offset behind the laser, so that we have some more distance between the origin and
    // the targets.
    private const float targetingOriginOffset = 0.7f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgPlayerTargeting), nameof(vgPlayerTargeting.UpdateTarget))]
    private static void UserLaserForTargeting(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
    {
        var laser = StageInstance.GetLaserTransform();
        if (!StageInstance.GetLaserTransform()) return;
        cameraFacing = laser.forward;
        cameraOrigin = laser.position - laser.forward * targetingOriginOffset;
    }
}