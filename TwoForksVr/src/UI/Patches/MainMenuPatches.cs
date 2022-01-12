using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public static class MainMenuPatches
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
    }
}