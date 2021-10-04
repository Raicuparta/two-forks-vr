using HarmonyLib;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVR.Settings.Patches
{
    [HarmonyPatch]
    public class SettingsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.headBob), MethodType.Setter)]
        public static void ForceDisableHeadBob(ref bool value)
        {
            value = false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgSettingsManager), nameof(vgSettingsManager.minimalInterface), MethodType.Setter)]
        public static void ForceEnableMinimalInterface(ref bool value)
        {
            value = true;
        }
    }
}