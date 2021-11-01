using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Input.Patches
{
    [HarmonyPatch]
    public static class InputPatches
    {
        private static SteamVR_Input_ActionSet_default actionSet;
        private static Dictionary<string, SteamVR_Action_Boolean> booleanActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2XActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2YActionMap;

        private static void Initialize()
        {
            actionSet = SteamVR_Actions._default;
            booleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
            {
                {InputName.LocomotionAction, actionSet.Interact},
                {InputName.DialogUp, actionSet.UIUp},
                {InputName.DialogDown, actionSet.UIDown},
                {InputName.Jog, actionSet.Jog},
                {InputName.Pause, actionSet.Cancel},
                {InputName.Use, actionSet.Interact},
                {InputName.NextMenu, actionSet.NextPage},
                {InputName.PreviousMenu, actionSet.PreviousPage}
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveXAxis, actionSet.Move},
                {InputName.LookYAxisStick, actionSet.Rotate}
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveYAxis, actionSet.Move},
                {InputName.LookYAxisStick, actionSet.Rotate}
            };

            // Pick dialog option with interact button.
            // TODO: move this somewhere else.
            actionSet.Interact.onStateDown += (fromAction, fromSource) =>
            {
                if (!vgDialogTreeManager.Instance) return;
                vgDialogTreeManager.Instance.OnConfirmDialogChoice();
                vgDialogTreeManager.Instance.ClearNonRadioDialogChoices();
            };
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgAxisData), nameof(vgAxisData.Update))]
        private static bool ReadAxisValuesFromSteamVR(vgAxisData __instance)
        {
            if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                return true;
            }
            if (!SteamVR_Input.initialized) return false;

            __instance.axisValueLastFrame = __instance.axisValue;
            
            if (actionSet == null) Initialize();

            foreach (var name in __instance.names)
            {
                if (vector2XActionMap.ContainsKey(name))
                {
                    __instance.axisValue = vector2XActionMap[name].axis.x;
                    __instance.axisValueLastFrame = vector2XActionMap[name].lastAxis.x;
                }
                else if (vector2YActionMap.ContainsKey(name))
                {
                    __instance.axisValue = vector2YActionMap[name].axis.y;
                    __instance.axisValueLastFrame = vector2YActionMap[name].lastAxis.y;
                }
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgButtonData), nameof(vgButtonData.Update))]
        private static bool ReadButtonValuesFromSteamVR(vgButtonData __instance)
        {
            if (UnityEngine.Input.GetKey(KeyCode.Space))
            {
                return true;
            }
            
            if (!SteamVR_Input.initialized) return false;
            
            // TODO leftover stuff (lastReleaseTime, lastHoldTime) from original method.

            if (actionSet == null) Initialize();

            foreach (var name in __instance.names.Where(name => booleanActionMap.ContainsKey(name)))
            {
                __instance.keyUp = booleanActionMap[name].stateUp;
                __instance.keyDown = booleanActionMap[name].stateDown;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            // TODO: could probably just use the streamingassets folder and avoid doing this?
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVR/Bindings";
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.Awake))]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.SetControllerLayout))]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.SetKeyBindFromPlayerPrefs))]
        private static void ForceXboxController(vgInputManager __instance)
        {
            __instance.currentControllerLayout = vgControllerLayoutChoice.XBox;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.GetLayout))]
        private static void ForceVrControllerLayout(vgControllerLayout __result)
        {
            if (__result.mapping == null) return;
            foreach (var keyCodeToVirtualKey in __result.mapping)
            {
                keyCodeToVirtualKey.keyCode = keyCodeToVirtualKey.virtualKey;
            }
        }
    }
}