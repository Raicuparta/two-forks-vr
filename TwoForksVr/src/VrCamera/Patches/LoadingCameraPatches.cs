using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.VrCamera.Patches;

[HarmonyPatch]
public class LoadingCameraPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDestroy))]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnDisable))]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.LateUpdate))]
    private static bool SkipLoadingCameraDisableAndDestroy()
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
    private static void CreateVrLoadingCamera(vgLoadingCamera __instance)
    {
        if (Object.FindObjectOfType<VrLoadingCamera>()) return;

        VrLoadingCamera.Create(__instance);
    }
}