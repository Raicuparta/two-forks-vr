using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class MainMenuPatches: TwoForksVrPatch
    {
        // These are all overlays that only made sense in pancake mode.
        private static readonly string[] objectsToDisable =
        {
            "BinocularParent",
            "OpeningFadeupSweetener",
            "MainMenuScrim"
        };

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void DisableUnnecessaryMainMenuObjects(vgMainMenuController __instance)
        {
            foreach (Transform child in __instance.transform)
                if (objectsToDisable.Contains(child.name))
                    child.gameObject.SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void FixMainMenuPosition(vgMainMenuController __instance)
        {
            var mainMenuGroup = __instance.transform.Find("Main Menu Group");
            mainMenuGroup.localPosition = new Vector3(0, mainMenuGroup.localPosition.y, 0);
        }
    }
}