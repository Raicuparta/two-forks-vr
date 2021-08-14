using System.Linq;
using HarmonyLib;
using TwoForksVR.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoForksVR.UI.Patches
{
    [HarmonyPatch]
    public static class UIPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), "ShowAbilityIcon")]
        private static bool PreventShowingAbilityIcon()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), "InitializeAbilityIcon")]
        private static bool DestroyAbilityIcon(vgHudManager __instance)
        {
            Object.Destroy(__instance.abilityIcon);
            return false;
        }
    }
    
    [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
    public static class DisablePauseBlur
    {
        public static void Prefix(ref bool blur)
        {
            blur = false;
        }
    }


    [HarmonyPatch]
    public static class MoveCanvasToWorldSpace
    {
        private static readonly string[] canvasesToDisable =
        {
            "BlackBars", // Cinematic black bars.
            "Camera" // Disposable camera. TODO: show this information in some other way.
        };

        private static readonly string[] canvasesToIgnore =
        {
            "ExplorerCanvas" // UnityExplorer.
        };

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIBehaviour), "Awake")]
        private static void UIBehaviourAwake(UIBehaviour __instance)
        {
            __instance.gameObject.layer = LayerFromName.UI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        private static void CanvasScalerEnable(CanvasScaler __instance)
        {
            PatchCanvases(__instance);
        }

        private static void PatchCanvases(Component component)
        {
            var camera = Camera.main;
            if (!camera || canvasesToIgnore.Contains(component.name)) return;

            component.gameObject.layer = LayerFromName.UI;
            var canvas = component.GetComponentInParent<Canvas>();

            if (!canvas) return;

            if (canvasesToDisable.Contains(canvas.name))
            {
                canvas.enabled = false;
                return;
            }
            
            var attachToCamera = canvas.GetComponent<AttachToCamera>();
            if (attachToCamera)
            {
                
            }

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) return;

            canvas.worldCamera = camera;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.gameObject.layer = LayerFromName.UI;
            canvas.gameObject.AddComponent<AttachToCamera>();
            canvas.transform.localScale = Vector3.one * 0.0005f;
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

            if (___menuCamera != null)
            {
                ___menuCamera.gameObject.SetActive(false);
            }
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