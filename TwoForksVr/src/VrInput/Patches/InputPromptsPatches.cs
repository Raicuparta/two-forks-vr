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
            __result = id; // TODO should just hide the prompt in this case?

            var action = StageInstance.GetInputAction(id);
            if (action == null)
            {
                Logs.LogWarning($"Failed to find actionInput for virtual key {id}");
                return false;
            }

            __result = action.Action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
                EVRInputStringBits.VRInputString_InputSource);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
        private static bool ReplacePromptIconsWithVrButtonText(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}