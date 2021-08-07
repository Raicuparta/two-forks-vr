using HarmonyLib;
using System.Linq;
using TwoForksVR.Hands;
using TwoForksVR.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVR.UI
{
    [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
    public class DisablePauseBlur
    {
        public static void Prefix(ref bool blur)
        {
            blur = false;
        }
    }

    [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
    public class MoveCanvasToWorldSpace
    {
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera", // Disposable camera. TODO: show this information in some other way.
        };
        private static readonly string[] canvasesToIgnore =
        {
            "ExplorerCanvas", // UnityExplorer.
        };

        public static void Prefix(CanvasScaler __instance)
        {
            if (!Camera.main || canvasesToIgnore.Contains(__instance.name))
            {
                return;
            }

            var canvas = __instance.GetComponent<Canvas>();

            if (!canvas)
            {
                TwoForksVRMod.LogError($"MoveCanvasToWorldSpace: {__instance.name} has no Canvas");
                return;
            }

            if (canvasesToDisable.Contains(canvas.name))
            {
                canvas.enabled = false;
                return;
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return;
            }

            canvas.worldCamera = Camera.main;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.gameObject.AddComponent<AttachToCamera>();
            __instance.transform.localScale = Vector3.one * 0.0005f;
        }
    }

    [HarmonyPatch(typeof(vgInventoryScreenController), "OnEnable")]
    public class PreventInventoryDisablingMainCamera
    {
        public static void Postfix(Camera ___mainCamera, Camera ___menuCamera)
        {
            if (___mainCamera != null)
            {
                ___mainCamera.enabled = true;
            }
            ___menuCamera?.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(vgInventoryScreenController), "Start")]
    public class InventoryFollowMainCamera
    {
        public static Transform RightHand;

        public static void Prefix(vgInventoryScreenController __instance)
        {
            if (RightHand == null)
            {
                TwoForksVRMod.LogError("Right hand transform hasn't been set up properly in InventoryFollowMainCamera patch");
                return;
            }

            var objectStage = __instance.transform.Find("ObjectStage").gameObject;
            if (objectStage.GetComponent<LateUpdateFollow>()) return;

            objectStage.AddComponent<LateUpdateFollow>().Target = RightHand;

            var inventoryObjectParent = objectStage.transform.Find("InventoryObjectParent");
            inventoryObjectParent.localPosition = new Vector3(-0.16f, -0.04f, 0f);
            inventoryObjectParent.localEulerAngles = new Vector3(328.5668f, 166.9781f, 334.8478f);

            objectStage.transform.Find("ObjectStageDirectionalLight").gameObject.SetActive(false);

            var footer = __instance.transform.Find("InventoryCanvas/SafeZoner/InventoryVerticalLayout/Menu Footer").gameObject;
            footer.SetActive(false);
        }
    }
}
