using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TwoForksVr.UI.Patches
{
    [HarmonyPatch]
    public class UIPatches : TwoForksVrPatch
    {
        private static readonly Dictionary<string, Material> materialMap = new Dictionary<string, Material>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.ShowAbilityIcon))]
        private static bool PreventShowingAbilityIcon()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.InitializeAbilityIcon))]
        private static bool DestroyAbilityIcon(vgHudManager __instance)
        {
            Object.Destroy(__instance.abilityIcon);
            return false;
        }

        // For some reason, the default text shader draws on top of everything.
        // I'm importing the TMPro shader from a more recent version and replacing it in the font materials.
        // This way, I can decide which ones I actually want to draw on top.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.Awake))]
        [HarmonyPatch(typeof(TextMeshProUGUI), nameof(TextMeshProUGUI.OnEnable))]
        private static void PreventTextFromDrawingOnTop(TextMeshProUGUI __instance)
        {
            try
            {
                var isInteractive = __instance.canvas && __instance.canvas.GetComponent<GraphicRaycaster>();
                var key = $"{__instance.font.name}{(isInteractive ? "interactive" : "non-interactive")}";

                materialMap.TryGetValue(key, out var material);

                if (material == null)
                {
                    material = new Material(__instance.font.material);
                    if (__instance.canvas && __instance.canvas.GetComponent<GraphicRaycaster>())
                        material.shader = VrAssetLoader.TMProShader;

                    materialMap[key] = material;
                }

                __instance.SetFontMaterial(material);
                __instance.SetSharedFontMaterial(material);
                __instance.SetFontBaseMaterial(material);

                // Problem: setting fontSharedMaterial is needed to prevent errors and the empty settings dropdowns,
                // but it also makes the dialog choices stop rendering on top.
                // __instance.fontSharedMaterial = material;
            }
            catch (Exception exception)
            {
                Logs.LogWarning($"Error in TMPro Patch ({__instance.name}): {exception}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgHudManager), nameof(vgHudManager.Awake))]
        private static void HideHudElements(vgHudManager __instance)
        {
            __instance.readObjectButtonGroup.transform.parent.Find("ExamineItem").gameObject.SetActive(false);
            __instance.readObjectButtonGroup.SetActive(false);

            // Dummy object is just so the hud manager still has a valid reference after we destroy the object.
            __instance.readObjectButtonGroup = new GameObject("Dummy");
            __instance.readObjectButtonGroup.transform.SetParent(__instance.transform, false);

            var safeZoner = __instance.transform.Find("uGUI Root/HUD/SafeZoner");
            var reticule = safeZoner.Find("ReticuleGroup/ReticuleParent/ReticuleCanvasGroup/Reticule");
            reticule.GetComponent<Image>().enabled = false;
            reticule.Find("ReticuleLarge").GetComponent<Image>().enabled = false;
        }
    }
}