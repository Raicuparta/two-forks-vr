using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace TwoForksVr.VrCamera.Patches;

[HarmonyPatch]
public class GameCameraPatches : TwoForksVrPatch
{
    private static bool isDone;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
    private static void RecenterCamera()
    {
        if (isDone) return;
        StageInstance.RecenterPosition(true);
        isDone = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.Start))]
    private static void ResetIsDoneOnCameraStart(vgCameraController __instance)
    {
        isDone = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgPlayerNavigationController), nameof(vgPlayerNavigationController.Start))]
    private static void FixFog(vgPlayerNavigationController __instance)
    {
        var stylisticFog = __instance.cameraController.camera.GetComponent<vgStylisticFog>();
        if (!stylisticFog || !stylisticFog.enabled) return;
        stylisticFog.enabled = false;
        stylisticFog.enabled = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
    private static void ResetIsDoneOnLoading(vgLoadingCamera __instance)
    {
        isDone = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraLimit), nameof(vgCameraLimit.SetLimits))]
    private static void PreventCameraVerticalRotation(ref float minVerticalAngle, ref float maxVerticalAngle)
    {
        minVerticalAngle = 0;
        maxVerticalAngle = 0;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgUtils), nameof(vgUtils.GetGameCamera))]
    private static bool ReplaceGetCameraResult(ref Camera __result)
    {
        var camera = StageInstance.GetMainCamera();
        if (!camera) return true;

        __result = camera;
        return false;
    }
}