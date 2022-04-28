using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using TwoForksVr.Helpers;
using TwoForksVr.VrInput;
using UnityEngine;

namespace TwoForksVr.UI.Patches;

[HarmonyPatch]
public static class TutorialPatches
{
    private static readonly Dictionary<string, string> tutorialTextMap = new()
    {
        {"textRadio", $"Hold [{VirtualKey.Radio}] to activate radio."},
        {"textRadio_select_dialog", $"[{VirtualKey.ScrollUpDown}] to select dialog."},
        {"textUse_radio", $"Release [{VirtualKey.Radio}] to talk to Delilah."},
        {"textInspect_object", "Look at hand to inspect objects."},
        {"textInspect_object_move", ""},
        {
            "textRadio_object",
            $"Aim with dominant hand and hold [{VirtualKey.Radio}] to talk about targeted object."
        },
        {"textJournal_examine", "Look at hand to inspect journal."},
        {"textJournal_stow", $"[{VirtualKey.StoreObject}] to keep journal."},
        {"textCompass", $"Hold [{VirtualKey.ToolPicker}] to select compass from tool picker."},
        {"textFlashlight", $"Hold [{VirtualKey.ToolPicker}] to select flashlight from tool picker."},
        {"textMap", $"Hold [{VirtualKey.ToolPicker}] to select map from tool picker."},
        {"textJog_toggle", $"Press [{VirtualKey.Jog}] to toggle jogging."},
        {"textMantle", $"Press [{VirtualKey.LocomotionAction}] to climb over obstructions."},
        {"textUse_object", $"Aim with dominant hand and press [{VirtualKey.Use}] to use objects."},
        {"textInventory_open", $"Hold [{VirtualKey.ToolPicker}] to select notes inventory from the tool picker."},
        {"textInventory_browse", "Use hand laser to select notes. Drag with laser to scroll."},
        {"textInventory_read", $"[{VirtualKey.StoreObject}] to store note"},
        {"textCamera", $"Hold [{VirtualKey.ToolPicker}] to select camera from tool picker."},
        {"textCamera_picture", $"Press [{VirtualKey.Use}] to take a picture."},
        {"textCamera_lower", $"Press [{VirtualKey.ToolPicker}] to lower camera."},
        {"textWaveReceiver", $"Hold [{VirtualKey.ToolPicker}] to select wave receiver from tool picker."},
        {"textRadio_concept", $"Hold [{VirtualKey.Radio}] to radio about current subject of interest."},
        {"textRadio_heldobject", $"Hold [{VirtualKey.Radio}] to talk about currently held object."},

        // There's no zooming in VR. Couldn't figure out an easy way to hide the whole tutorial box,
        // so I'm just showing the jogging tutorial instead.
        {"textZoom", $"Press [{VirtualKey.Jog}] to toggle jogging."},

        // Unused tutorial text.
        {"Text", ""}
    };

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
    private static void SetUpTutorials(vgHudManager __instance)
    {
        var tutorialObject =
            __instance.transform.Find(
                "uGUI Root/HUD/SafeZoner/BottomCenterGroup/TutorialPopup/TutorialPopupParent/TutorialObject");

        Logs.WriteInfo($"Found {tutorialObject.childCount} tutorials");
        foreach (Transform child in tutorialObject)
        {
            Logs.WriteInfo($"{child.name}: {child.GetComponent<TextMeshProUGUI>().text}");
            tutorialTextMap.TryGetValue(child.name, out var tutorialString);
            if (tutorialString == null)
            {
                Logs.WriteError($"No tutorial text found for {child.name}");
                continue;
            }

            child.GetComponent<TextMeshProUGUI>().text = tutorialString;
        }
    }
}