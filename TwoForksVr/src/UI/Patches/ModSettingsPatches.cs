using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public static class ModSettingsPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void CreateModSettingsMainMenuButton(vgMainMenuController __instance)
        {
            var mainMenuGroup = __instance.transform.Find("Main Menu Group");
            var gameSettingsButton = mainMenuGroup.Find("Settings Button").GetComponent<Button>();
            var specialFeaturesButton = mainMenuGroup.Find("Special Features Button").GetComponent<Button>();

            var vrSettingsButton = Object.Instantiate(gameSettingsButton, mainMenuGroup, false);

            var gameSettingsButtonNavigation = gameSettingsButton.navigation;
            gameSettingsButtonNavigation.selectOnDown = vrSettingsButton;
            gameSettingsButton.navigation = gameSettingsButtonNavigation;
            
            var vrSettingsButtonNavigation = vrSettingsButton.navigation;
            vrSettingsButtonNavigation.selectOnUp = gameSettingsButton;
            vrSettingsButtonNavigation.selectOnDown = specialFeaturesButton;
            vrSettingsButton.navigation = vrSettingsButtonNavigation;
            
            var specialFeaturesButtonNavigation = specialFeaturesButton.GetComponent<vgButtonNavigationFixer>();
            specialFeaturesButtonNavigation.selectOnUp = vrSettingsButton;
            // Trigger enable in vgButtonNavigationFixer to make it update the navigation.
            specialFeaturesButtonNavigation.OnEnable();
            
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
            var subtitlesCheckboxNavigation = subtitlesCheckbox.navigation;
            subtitlesCheckboxNavigation.selectOnUp = vrSettingsButton;
            subtitlesCheckbox.navigation = subtitlesCheckboxNavigation;
        }
    }
}