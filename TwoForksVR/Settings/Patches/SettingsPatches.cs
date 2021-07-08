using Harmony;

// Some of the available game settings don't go well with VR.
// These patches force some settings to certain values to prevent VR funkyness.
namespace TwoForksVR.Settings
{
    [HarmonyPatch(typeof(vgSettingsManager), "headBob", MethodType.Setter)]
    public class ForceDisableHeadBob
    {
        public static void Prefix(ref bool value)
        {
            value = false;
        }
    }

    [HarmonyPatch(typeof(vgSettingsManager), "minimalInterface", MethodType.Setter)]
    public class ForceEnableMinimalInterface
    {
        public static void Prefix(ref bool value)
        {
            value = true;
        }
    }
}
