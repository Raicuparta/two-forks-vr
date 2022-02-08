using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using TwoForksVr.Helpers;
using TwoForksVr.VrInput;
using UnityEngine;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public static class TutorialPatches
    {
        private static readonly Dictionary<string, string> tutorialTextMap = new Dictionary<string, string>
        {
            {"textRadio", "Hold [RadioButton] to activate radio."},
            {"textRadio_select_dialog", "[ScrollUpDown] to select dialog."},
            {"textUse_radio", "Release [RadioButton] to talk to Delilah."},
            {"textInspect_object", "Look at hand to inspect objects."},
            {"textInspect_object_move", ""},
            {"textRadio_object", "Aim with hand and hold [RadioButton] to talk about targeted object."},
            {"textJournal_examine", "Look at hand to inspect journal."},
            {"textJournal_stow", "[StoreObjectButton] to keep journal."},
            {"textCompass", $"Hold [{VirtualKey.ToolPicker}] to select compass from tool picker."},
            // {"textZoom", "Hold [ZoomButton] to zoom."},
            {"textZoom", ""},
            {"textFlashlight", $"Hold [{VirtualKey.ToolPicker}] to select flashlight from tool picker."},
            {"textMap", $"Hold [{VirtualKey.ToolPicker}] to select map from tool picker."},
            {"textJog_toggle", "[JogButton] to toggle jogging."},
            {"textMantle", "[LocomotionActionButton] to climb over obstructions."},
            {"textUse_object", "Aim with hand and press [UseButton] to use objects."},
            {"textInventory_open", $"Hold [{VirtualKey.ToolPicker}] to select notes inventory from the tool picker."},
            {"textInventory_browse", "Use hand laser to browse notes."},
            // {"textInventory_read", "[ReadModeButton] to read note"},
            {"textInventory_read", ""},
            {"textCamera", $"Hold [{VirtualKey.ToolPicker}] to select camera from tool picker."},
            {"textCamera_picture", "[UseButton] to take a picture."},
            {"textCamera_lower", $"Press [{VirtualKey.ToolPicker}] to lower camera."},
            {"textWaveReceiver", $"Hold [{VirtualKey.ToolPicker}] to select wave receiver from tool picker."},
            {"textRadio_concept", "Hold [RadioButton] to radio about current subject of interest."},
            {"textRadio_heldobject", "Hold [RadioButton] to talk about currently held object."}
        };

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
        private static void SetUpTutorials(vgHudManager __instance)
        {
            var tutorialObject =
                __instance.transform.Find(
                    "uGUI Root/HUD/SafeZoner/BottomCenterGroup/TutorialPopup/TutorialPopupParent/TutorialObject");

            Logs.LogInfo($"Found {tutorialObject.childCount} tutorials");
            foreach (Transform child in tutorialObject)
            {
                Logs.LogInfo($"{child.name}: {child.GetComponent<TextMeshProUGUI>().text}");
                tutorialTextMap.TryGetValue(child.name, out var tutorialString);
                if (tutorialString == null)
                {
                    Logs.LogError($"No tutorial text found for {child.name}");
                    continue;
                }

                child.GetComponent<TextMeshProUGUI>().text = tutorialString;
            }
        }
    }
}