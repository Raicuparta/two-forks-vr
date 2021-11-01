using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using TwoForksVR.Helpers;
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
                // {InputName.LocomotionAction, actionSet.Interact},
                {InputName.Use, actionSet.Interact},
                // {InputName.Confirm, actionSet.Interact},
                // {InputName.DialogUp, actionSet.UIUp},
                // {InputName.DialogDown, actionSet.UIDown},
                {InputName.ToggleJog, actionSet.Jog},
                {InputName.Pause, actionSet.Cancel},
                // {InputName.NextMenu, actionSet.NextPage},
                // {InputName.PreviousMenu, actionSet.PreviousPage}
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveStrafe, actionSet.Move},
                {InputName.LookHorizontalStick, actionSet.Rotate},
                // {InputName.UILeftStickHorizontal, actionSet.Move},
                // {InputName.UIRightStickHorizontal, actionSet.Rotate}
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveForward, actionSet.Move},
                {InputName.LookVerticalStick, actionSet.Rotate},
                // {InputName.ScrollUpDown, actionSet.Move},
                // {InputName.UILeftStickVertical, actionSet.Move},
                // {InputName.UIRightStickVertical, actionSet.Rotate}
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
        [HarmonyPatch(typeof(vgInputManager), nameof(vgInputManager.BuildActiveKeyBinds))]
        private static void ForceVrControllerLayout(vgInputManager __instance)
        {
            Logs.LogInfo("## starting inputs");
            foreach (var bind in __instance.virtualKeyKeyBindMap.Values)
            {
                if (bind.keyData.names.Count == 0 || bind.commands.Count == 0) continue;
                Logs.LogInfo($"{bind.commands[0]}");
                bind.keyData.names[0] = bind.commands[0].command;
            }
        }
    }
}