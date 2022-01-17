using HarmonyLib;
using TMPro;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public static class ModSettingsPatches
    {
        private static void SetNavigation(Selectable selectable, Selectable selectOnUp = null, Selectable selectOnDown = null)
        {
            if (!selectable) return;
            var navigationFixer = selectable.GetComponent<vgButtonNavigationFixer>();
            if (navigationFixer) SetNavigationWithFixer(navigationFixer, selectOnUp, selectOnDown);
            else SetNavigationWithoutFixer(selectable, selectOnUp, selectOnDown);
        }

        private static void SetNavigationWithFixer(vgButtonNavigationFixer navigationFixer, Selectable selectOnUp = null, Selectable selectOnDown = null)
        {
            navigationFixer.selectOnUp = selectOnUp;
            navigationFixer.selectOnDown = selectOnDown;
            
            // Triggering OnEnable required to apply changes in navigation fixer.
            navigationFixer.OnEnable();
        }
        
        private static void SetNavigationWithoutFixer(Selectable selectable, Selectable selectOnUp = null, Selectable selectOnDown = null)
        {
            var navigation = selectable.navigation;
            if (navigation.mode != Navigation.Mode.Explicit) return;
            if (selectOnUp) navigation.selectOnUp = selectOnUp;
            if (selectOnDown) navigation.selectOnDown = selectOnDown;
            selectable.navigation = navigation;
        }
        
        private static Selectable GetNavigationDown(Selectable selectable)
        {
            var navigationFixer = selectable.GetComponent<vgButtonNavigationFixer>();
            if (navigationFixer) return navigationFixer.selectOnDown;

            var navigation = selectable.navigation;
            return navigation.mode == Navigation.Mode.Explicit ? navigation.selectOnDown : null;
        }

        private static Button CreateVrSettingsButton(Button buttonAbove, Transform parent)
        {
            var vrSettingsButton = CreateButton(buttonAbove, parent, "VR Settings");
            
            var vrSettingsMenu = VrSettingsMenu.Create(VrStage.Instance); // TODO don't pass singleton
            vrSettingsMenu.gameObject.SetActive(false);
            RemoveAllClickListeners(vrSettingsButton);
            vrSettingsButton.onClick.AddListener(() =>
            {
                vrSettingsMenu.gameObject.SetActive(true);
            });

            return vrSettingsButton;
        }
        
        private static Button CreateButton(Button buttonAbove, Transform parent, string name)
        {
            var newButton = Object.Instantiate(buttonAbove, parent, false);
            newButton.transform.SetSiblingIndex(buttonAbove.transform.GetSiblingIndex() + 1);
            newButton.name = name;
            newButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = name;
            
            var buttonBelow = GetNavigationDown(buttonAbove);
            SetNavigation(buttonAbove, null, newButton);
            SetNavigation(newButton, buttonAbove, buttonBelow);
            SetNavigation(buttonBelow, newButton);
            
            return newButton;
        }

        private static void RemoveAllClickListeners(Button button)
        {
            button.onClick.RemoveAllListeners();

            // "Persistent" events are the ones set in the Unity editor. They don't get removed by RemoveAllListeners.
            for (var index = 0; index < button.onClick.GetPersistentEventCount(); index++)
            {
                button.onClick.SetPersistentListenerState(index, UnityEventCallState.Off);
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void CreateModSettingsMainMenuButton(vgMainMenuController __instance)
        {
            var mainMenuGroup = __instance.transform.Find("Main Menu Group");
            var gameSettingsButton = mainMenuGroup.Find("Settings Button").GetComponent<Button>();
            
            CreateVrSettingsButton(gameSettingsButton, mainMenuGroup);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
        private static void CreateModSettingsPauseMenuButton(vgHudManager __instance)
        {
            var settingsMenuGroup = __instance.transform.Find("PauseRoot/Pause Canvas/SafeZoner/Settings Menu Group");
            var gameSettingsButton = settingsMenuGroup.Find("ButtonSettings").GetComponent<Button>();
            var vrSettingsButton = CreateVrSettingsButton(gameSettingsButton, settingsMenuGroup);
            var subtitlesCheckbox = settingsMenuGroup.Find("Subtitles Checkbox").GetComponent<Toggle>();
            SetNavigation(subtitlesCheckbox, vrSettingsButton);
        }
    }
}