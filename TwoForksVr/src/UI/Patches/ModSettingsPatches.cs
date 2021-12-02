using HarmonyLib;
using TMPro;
using UnityEngine;
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

        private static Selectable GetNavigationUp(Selectable selectable)
        {
            var navigationFixer = selectable.GetComponent<vgButtonNavigationFixer>();
            if (navigationFixer) return navigationFixer.selectOnUp;

            var navigation = selectable.navigation;
            return navigation.mode == Navigation.Mode.Explicit ? navigation.selectOnUp : null;
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
            return CreateButton(buttonAbove, parent, "VR Settings");
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