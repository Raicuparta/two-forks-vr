using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches;

[HarmonyPatch]
public class CustomMenuItemPatches : TwoForksVrPatch
{
    private static Button AddMenuButton(Transform parent, Button buttonAbove, string text)
    {
        var button = Object.Instantiate(buttonAbove, parent, false);
        button.transform.SetSiblingIndex(buttonAbove.transform.GetSiblingIndex() + 1);
        button.name = text;
        button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
        RemoveAllClickListeners(button);

        return button;
    }

    private static void RemoveAllClickListeners(Button button)
    {
        button.onClick.RemoveAllListeners();

        // "Persistent" events are the ones set in the Unity editor. They don't get removed by RemoveAllListeners.
        for (var index = 0; index < button.onClick.GetPersistentEventCount(); index++)
            button.onClick.SetPersistentListenerState(index, UnityEventCallState.Off);
    }

    private static void CreateVrSettingsButton(Button buttonAbove, Transform parent)
    {
        var vrSettingsButton = AddMenuButton(parent, buttonAbove, "VR Settings");

        vrSettingsButton.onClick.AddListener(StageInstance.OpenVrSettings);
    }

    private static void CreateRecenterButton(Button buttonAbove, Transform parent)
    {
        var recenterButton = AddMenuButton(parent, buttonAbove, "Recenter VR");

        recenterButton.onClick.AddListener(StageInstance.Recenter);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
    private static void CreateVrSettingsMainMenuButton(vgMainMenuController __instance)
    {
        var mainMenuGroup = __instance.transform.Find("Main Menu Group");
        var gameSettingsButton = mainMenuGroup.Find("Settings Button").GetComponent<Button>();

        CreateVrSettingsButton(gameSettingsButton, mainMenuGroup);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
    private static void CreateVrSettingsPauseMenuButton(vgHudManager __instance)
    {
        var settingsMenuGroup = __instance.transform.Find("PauseRoot/Pause Canvas/SafeZoner/Settings Menu Group");
        var gameSettingsButton = settingsMenuGroup.Find("ButtonSettings").GetComponent<Button>();

        CreateRecenterButton(gameSettingsButton, settingsMenuGroup);
        CreateVrSettingsButton(gameSettingsButton, settingsMenuGroup);
    }
}