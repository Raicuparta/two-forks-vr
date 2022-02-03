using HarmonyLib;
using TMPro;
using TwoForksVr.Helpers;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class InputPromptsPatches : TwoForksVrPatch
    {
        // Todo this shouldnt be here I guess.
        private static string currentButtonDisplay;

        private static void ResetPrompt()
        {
            currentButtonDisplay = "";
            StageInstance.HighlightButton();
        }

        // TODO old controller model code.
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.UpdateButtonText))]
        // private static bool TriggerControllerButtonHighlight(vgHudManager __instance, string buttonDisplay,
        //     TextMeshProUGUI buttonText)
        // {
        //     if (__instance.currentTarget && !__instance.currentTarget.ShouldDisplayDetail())
        //     {
        //         ResetPrompt();
        //         return false;
        //     }
        //
        //     if (buttonDisplay == currentButtonDisplay) return false;
        //
        //     currentButtonDisplay = buttonDisplay;
        //
        //     var virtualKey = buttonDisplay.Trim('[', ']');
        //     vgInputManager.Instance.virtualKeyKeyBindMap.TryGetValue(virtualKey, out var keyBind);
        //
        //     if (keyBind == null)
        //     {
        //         Logs.LogWarning($"Failed to find keyBind for buttonDisplay {buttonDisplay}");
        //         return false;
        //     }
        //
        //     foreach (var command in keyBind.commands)
        //     {
        //         var action = StageInstance.GetBooleanAction(command.command);
        //         if (action == null) continue;
        //         StageInstance.HighlightButton(action);
        //         buttonText.text = action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
        //             EVRInputStringBits.VRInputString_InputSource);
        //         buttonText.gameObject.SetActive(true);
        //         // Doing it only for the first command that works, not sure if that canb e a problem.
        //         break;
        //     }
        //
        //     return false;
        // }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ClearButtonText))]
        private static bool StopControllerButtonHighlight(TextMeshProUGUI buttonText)
        {
            if (!buttonText.gameObject.activeSelf) return false;
            buttonText.gameObject.SetActive(false);
            ResetPrompt();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.GetIconName))]
        private static bool ReplacePromptIconsWithVrButtonText(ref string __result, string id)
        {
            __result = id; // TODO should just hide the prompt in this case?
            vgInputManager.Instance.virtualKeyKeyBindMap.TryGetValue(id, out var keyBind);
            if (keyBind == null)
            {
                Logs.LogWarning($"Failed to find key bind for virtual key {id}");
                return false;
            }

            if (keyBind.commands.Count == 0)
            {
                Logs.LogWarning($"keybind {id} is empty");
                Logs.LogInfo($"names: {keyBind.keyData.names.Join()}");
                Logs.LogInfo($"virtualKeyNames: {keyBind.keyData.virtualKeyNames.Join()}");
                return false;
            }

            foreach (var command in keyBind.commands)
            {
                Logs.LogInfo($"Looking for friendly name for command {command.command}");
                var action = StageInstance.GetInputAction(command.command);
                if (action == null) continue;
                __result = action.Action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
                    EVRInputStringBits.VRInputString_InputSource);

                // Doing it only for the first command that works, not sure if that canb e a problem.
                return false;
            }

            Logs.LogWarning($"Failed to find friendly name for virtual key {id}");

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonIconMap), nameof(vgButtonIconMap.HasIcon))]
        private static bool ReplacePromptIconsWithVrButtonText(ref bool __result)
        {
            __result = true;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.UpdateEdgeHUD))]
        private static void ClearEdgeText(vgHudManager __instance)
        {
            if (!__instance.edgeObject.activeSelf)
                __instance.ClearButtonText(__instance.edgeButton, __instance.edgeButtonKeyBoundary);
        }

        // Patching GetIconName was an easier way to replace the input prompt text,
        // but for some reason I have to force IsHandlingInput to be true for GetIconName to be called.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.IsHandlingInput))]
        private static void ForceToUseGetIconName(ref bool __result)
        {
            __result = true;
        }
    }
}