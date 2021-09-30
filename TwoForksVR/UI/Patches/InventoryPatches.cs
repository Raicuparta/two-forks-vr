using HarmonyLib;
using TwoForksVR.Helpers;
using UnityEngine;

namespace TwoForksVR.UI.Patches
{
    [HarmonyPatch]
    public static class InventoryPatches
    {
        public static Transform RightHand;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgInventoryScreenController), "OnEnable")]
        private static void PreventInventoryDisablingMainCamera(Camera ___mainCamera, Camera ___menuCamera)
        {
            if (___mainCamera != null) ___mainCamera.enabled = true;

            if (___menuCamera != null) ___menuCamera.gameObject.SetActive(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgInventoryScreenController), "Start")]
        private static void InventoryFollowMainCamera(vgInventoryScreenController __instance)
        {
            if (RightHand == null)
            {
                TwoForksVRMod.LogError(
                    "Right hand transform hasn't been set up properly in InventoryFollowMainCamera patch");
                return;
            }

            var objectStage = __instance.transform.Find("ObjectStage").gameObject;
            if (objectStage.GetComponent<LateUpdateFollow>()) return;

            objectStage.AddComponent<LateUpdateFollow>().Target = RightHand;

            var inventoryObjectParent = objectStage.transform.Find("InventoryObjectParent");
            inventoryObjectParent.localPosition = new Vector3(-0.16f, -0.04f, 0f);
            inventoryObjectParent.localEulerAngles = new Vector3(328.5668f, 166.9781f, 334.8478f);

            objectStage.transform.Find("ObjectStageDirectionalLight").gameObject.SetActive(false);

            var footer = __instance.transform.Find("InventoryCanvas/SafeZoner/InventoryVerticalLayout/Menu Footer")
                .gameObject;
            footer.SetActive(false);
        }
    }
}