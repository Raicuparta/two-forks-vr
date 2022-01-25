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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.UpdateButtonText))]
        private static bool TriggerControllerButtonHighlight(vgHudManager __instance, string buttonDisplay,
            TextMeshProUGUI buttonText)
        {
            if (__instance.currentTarget && !__instance.currentTarget.ShouldDisplayDetail())
            {
                ResetPrompt();
                return false;
            }

            if (buttonDisplay == currentButtonDisplay) return false;

            currentButtonDisplay = buttonDisplay;

            var virtualKey = buttonDisplay.Trim('[', ']');
            vgInputManager.Instance.virtualKeyKeyBindMap.TryGetValue(virtualKey, out var keyBind);

            if (keyBind == null)
            {
                Logs.LogWarning($"Failed to find keyBind for buttonDisplay {buttonDisplay}");
                return false;
            }

            foreach (var command in keyBind.commands)
            {
                BindingsPatches.BooleanActionMap.TryGetValue(command.command, out var action);
                if (action != null)
                {
                    StageInstance.HighlightButton(action);
                    buttonText.text = action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any,
                        EVRInputStringBits.VRInputString_InputSource);
                    buttonText.gameObject.SetActive(true);
                    // Doing it only for the first command that works, not sure if that canb e a problem.
                    break;
                }
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ClearButtonText))]
        private static bool StopControllerButtonHighlight(TextMeshProUGUI buttonText)
        {
            if (!buttonText.gameObject.activeSelf) return false;
            buttonText.gameObject.SetActive(false);
            ResetPrompt();
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.UpdateEdgeHUD))]
        private static void ClearEdgeText(vgHudManager __instance)
        {
            if (!__instance.edgeObject.activeSelf)
                __instance.ClearButtonText(__instance.edgeButton, __instance.edgeButtonKeyBoundary);
        }
    }
}