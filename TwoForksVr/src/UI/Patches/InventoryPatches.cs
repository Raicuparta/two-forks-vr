using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using UnityEngine;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class InventoryPatches : TwoForksVrPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgInventoryScreenController), nameof(vgInventoryScreenController.OnEnable))]
        private static void PreventInventoryDisablingMainCamera(vgInventoryScreenController __instance)
        {
            if (__instance.mainCamera != null) __instance.mainCamera.enabled = true;

            if (__instance.menuCamera != null) __instance.menuCamera.gameObject.SetActive(false);

            var objectStage = __instance.transform.Find("ObjectStage").gameObject;
            objectStage.transform.Find("ObjectStageDirectionalLight").gameObject.SetActive(false);

            var footerButtons = __instance.transform
                .Find("InventoryCanvas/SafeZoner/InventoryVerticalLayout/Menu Footer/UI_TooltipBar")
                .gameObject;
            footerButtons.SetActive(false);

            var background = __instance.transform.Find(
                    "InventoryCanvas/SafeZoner/InventoryVerticalLayout/Menus/NotesMenu/MenuCanvasGroup/ReadingInterface/ReadingInterfaceBackground")
                .gameObject;
            background.SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgInventoryScreenController), nameof(vgInventoryScreenController.Start))]
        private static void InventoryFollowMainCamera(vgInventoryScreenController __instance)
        {
            var dominantHand = StageInstance.GetDominantHand();

            if (dominantHand == null)
            {
                Logs.LogError(
                    "Right hand transform hasn't been set up properly in InventoryFollowMainCamera patch");
                return;
            }

            var objectStage = __instance.transform.Find("ObjectStage").gameObject;
            if (objectStage.GetComponent<FakeParenting>()) return;

            FakeParenting.Create(objectStage.transform, dominantHand);

            var inventoryObjectParent = objectStage.transform.Find("InventoryObjectParent");
            // inventoryObjectParent.localPosition = new Vector3(0.16f, -0.04f, 0f);
            VrHandednessTransformModifier.Create(inventoryObjectParent,
                new Vector3(-0.16f, -0.04f, 0f),
                new Vector3(0.14f, 0.06f, -0.08f));
            inventoryObjectParent.localEulerAngles = new Vector3(328.5668f, 166.9781f, 334.8478f);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgNotesMenuController), nameof(vgNotesMenuController.SetSelectedInventory))]
        private static void ForceNotesReadingMode(vgNotesMenuController __instance)
        {
            __instance.reading = true;
        }
    }
}