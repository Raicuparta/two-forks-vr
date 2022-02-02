using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class MainMenuPatches : TwoForksVrPatch
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgMainMenuController), nameof(vgMainMenuController.Start))]
        private static void AddMainMenuBackground(vgMainMenuController __instance)
        {
            var background = __instance.transform.Find("Background Layout");
            background.gameObject.SetActive(true);
            var image = background.GetComponentInChildren<RawImage>();
            image.texture = null;
            image.color = new Color(0, 0, 0, 0.75f);
            if (image.transform.localPosition.z == 0) image.transform.localPosition += Vector3.forward * 50;
        }
    }
}