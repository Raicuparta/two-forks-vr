using System.Globalization;
using HarmonyLib;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class InputPromptsPatches : TwoForksVrPatch
    {
        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.GetIconName))]
        private static bool ReplacePromptIconsWithVrButtonText(ref string __result, string id)
        {
            var inputAction = StageInstance.GetInputAction(id);
            if (inputAction?.Action == null || !inputAction.Action.active)
            {
                __result = "n/a";
                return false;
            }

            var source = inputAction.ActiveSource;
            var hand = "";
            if (source == SteamVR_Input_Sources.RightHand) hand = "right ";
            if (source == SteamVR_Input_Sources.LeftHand) hand = "left ";

            var name = inputAction.Action.GetRenderModelComponentName(inputAction.ActiveSource);
            name = name.Replace("button_", "");
            name = name.Replace("thumb", "");
            __result = textInfo.ToTitleCase($"{hand}{name}{inputAction.PromptSuffix}");

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
        private static bool CheckHasIconFromVrInputs(ref bool __result, string id)
        {
            __result = true;
            return false;
        }
    }
}