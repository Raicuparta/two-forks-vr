using HarmonyLib;
using TwoForksVr.Helpers;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class InputPromptsPatches : TwoForksVrPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.GetIconName))]
        private static bool ReplacePromptIconsWithVrButtonText(ref string __result, string id)
        {
            var inputAction = StageInstance.GetInputAction(id);
            if (inputAction == null)
            {
                Logs.LogWarning($"Failed to find actionInput for virtual key {id}");
                return true;
            }

            __result = inputAction.Action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
                EVRInputStringBits.VRInputString_InputSource);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
        private static bool ReplacePromptIconsWithVrButtonText(ref bool __result, string id)
        {
            __result = StageInstance.GetInputAction(id) != null;
            return false;
        }
    }
}