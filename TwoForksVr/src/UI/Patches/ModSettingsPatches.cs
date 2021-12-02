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
    }
}