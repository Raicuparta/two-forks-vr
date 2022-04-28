using HarmonyLib;
using UnityEngine;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVr.Settings.Patches;

[HarmonyPatch]
public class GameSettingsPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.headBob), MethodType.Setter)]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.invertY), MethodType.Setter)]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.triggerSwap), MethodType.Setter)]
    private static void ForceDisableBoolSetting(ref bool value)
    {
        value = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.MotionBlurQuality), MethodType.Setter)]
    private static void ForceNoMotionBlur(ref int value)
    {
        // they use an int and then convert it into an enum later, pain
        value = 0;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.SetResolution),
        typeof(Resolution), typeof(bool))]
    private static bool PreventChangingResolution()
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgResolutionListController), nameof(vgResolutionListController.BuildList))]
    private static bool HideResolutionOptions()
    {
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(vgSettingsMenuController), nameof(vgSettingsMenuController.Start))]
    private static void HideRedundantSettingsMenuItems(vgSettingsMenuController __instance)
    {
        var safeZoner = __instance.transform.Find("SafeZoner");

        var headBob =
            safeZoner.Find(
                "SettingsOptionsRoot/CanvasGroup/SettingsVerticalList/GameplayGrid/GameplayLeftColumn/Head Bob Checkbox");
        headBob.gameObject.SetActive(false);

        var graphicsCanvasGroup = safeZoner.Find("GFXOptionsRoot/CanvasGroup/");

        var graphicsLeftColumn = graphicsCanvasGroup.Find("LeftColumn");
        graphicsLeftColumn.gameObject.SetActive(false);

        var motionBlur = graphicsCanvasGroup.Find(
            "RightColumn/UI_ScrollingContentArea GFXQuality/ScrollContents/ContentsContainer/MotionBlurOption");
        motionBlur.gameObject.SetActive(false);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.Awake))]
    private static void OverrideDefaultSettings(vgSettingsManager __instance)
    {
        __instance._BloomQuality = 0;
        __instance._LightShaftQuality = 0;
        __instance._SSAOQuality = 0;
        __instance._shadowQuality = 2;
        __instance._detailDistance = 2;
    }
}