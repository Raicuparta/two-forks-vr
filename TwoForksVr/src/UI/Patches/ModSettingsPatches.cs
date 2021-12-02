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
            if (selectOnUp) navigation.selectOnUp = selectOnUp;
            if (selectOnDown) navigation.selectOnDown = selectOnDown;
            selectable.navigation = navigation;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void CreateModSettingsMainMenuButton(vgMainMenuController __instance)
        {
            var mainMenuGroup = __instance.transform.Find("Main Menu Group");
            var gameSettingsButton = mainMenuGroup.Find("Settings Button").GetComponent<Button>();
            var specialFeaturesButton = mainMenuGroup.Find("Special Features Button").GetComponent<Button>();

            var vrSettingsButton = Object.Instantiate(gameSettingsButton, mainMenuGroup, false);

            SetNavigation(gameSettingsButton, null, vrSettingsButton);
            SetNavigation(vrSettingsButton, gameSettingsButton, specialFeaturesButton);
            SetNavigation(specialFeaturesButton, vrSettingsButton);

            vrSettingsButton.transform.SetSiblingIndex(gameSettingsButton.transform.GetSiblingIndex() + 1);
            vrSettingsButton.name = "VR Settings Button";
            vrSettingsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "VR Settings";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
        private static void CreateModSettingsPauseMenuButton(vgHudManager __instance)
        {
            var settingsMenuGroup = __instance.transform.Find("PauseRoot/Pause Canvas/SafeZoner/Settings Menu Group");
            var gameSettingsButton = settingsMenuGroup.Find("ButtonSettings").GetComponent<Button>();
            
            var vrSettingsButton = Object.Instantiate(gameSettingsButton, settingsMenuGroup, false);
            vrSettingsButton.transform.SetSiblingIndex(gameSettingsButton.transform.GetSiblingIndex() + 1);
            vrSettingsButton.name = "VR Settings Button";
            vrSettingsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "VR Settings";

            var subtitlesCheckbox = settingsMenuGroup.Find("Subtitles Checkbox").GetComponent<Toggle>();
            SetNavigation(subtitlesCheckbox, vrSettingsButton);
        }
    }
}