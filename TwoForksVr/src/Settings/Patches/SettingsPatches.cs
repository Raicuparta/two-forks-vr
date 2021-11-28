using HarmonyLib;
using UnityEngine;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVr.Settings.Patches
{
    [HarmonyPatch]
    public class SettingsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.headBob), MethodType.Setter)]
        private static void ForceDisableHeadBob(ref bool value)
        {
            value = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.minimalInterface), MethodType.Setter)]
        private static void ForceEnableMinimalInterface(ref bool value)
        {
            value = true;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.SetResolution), typeof(Resolution), typeof(bool))]
        private static void ForceResolution(ref Resolution newResolution, ref bool newFullscreen)
        {
            newResolution = new Resolution() { width = 1920, height = 1080 };
            newFullscreen = false;
        }
    }
}
