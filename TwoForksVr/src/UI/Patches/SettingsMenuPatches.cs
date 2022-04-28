using HarmonyLib;
using UnityEngine;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVr.UI.Patches;

[HarmonyPatch]
public class SettingsMenuPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgSettingsMenuController), nameof(vgSettingsMenuController.Start))]
    private static void RemoveUnusedSettingsElements(vgSettingsMenuController __instance)
    {
        // Rmove control settings screen.
        __instance.screens.RemoveAt(2);

        // Remove control settings tab.
        Object.Destroy(__instance.selectionGroup.buttonElements[2].gameObject);

        // Remove one of the tab input prompts.
        // Since the input prompts have been patched to use VR control names, both of these prompts (left and right)
        // were saying the same thing (something like "Right Stick"). So we can remove one of them.
        __instance.selectionGroup.transform.Find("ButtonContainer").GetChild(0).gameObject.SetActive(false);
    }
}