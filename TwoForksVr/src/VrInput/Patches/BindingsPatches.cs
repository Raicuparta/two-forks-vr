using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using TwoForksVr.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public static class BindingsPatches
    {
        private static SteamVR_Input_ActionSet_default actionSet;
        private static Dictionary<string, SteamVR_Action_Boolean> booleanActionMap;
        private static Dictionary<string, SteamVR_Action_Boolean> invertedBooleanActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2XActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2YActionMap;

        private static void Initialize()
        {
            actionSet = SteamVR_Actions._default;
            booleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
            {
                {InputName.LocomotionAction, actionSet.Interact},
                {InputName.UseCamera, actionSet.Interact},
                {InputName.Use, actionSet.Interact},
                {InputName.UISubmit, actionSet.Interact},
                {InputName.StowHeldObject, actionSet.Jog},
                {InputName.UICancel, actionSet.Cancel},
                {InputName.DialogSelectionUp, actionSet.UIUp},
                {InputName.DialogSelectionDown, actionSet.UIDown},
                {InputName.UIUp, actionSet.UIUp},
                {InputName.UIDown, actionSet.UIDown},
                {InputName.LockNumberUp, actionSet.UIUp},
                {InputName.LockNumberDown, actionSet.UIDown},
                {InputName.LockTumblerRight, actionSet.NextPage},
                {InputName.LockTumblerLeft, actionSet.PreviousPage},
                {InputName.LockCancel, actionSet.Cancel},
                {InputName.ToggleJog, actionSet.Jog},
                {InputName.Pause, actionSet.Cancel},
                {InputName.NextMenu, actionSet.NextPage},
                {InputName.PreviousMenu, actionSet.PreviousPage},
                {InputName.RadioUp, actionSet.Grip}
            };
            invertedBooleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
            {
                {InputName.RadioDown, actionSet.Grip}
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveStrafe, actionSet.Move},
                {InputName.LookHorizontalStick, actionSet.Rotate},
                {InputName.UIHorizontal, actionSet.Move}
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveForward, actionSet.Move},
                {InputName.LookVerticalStick, actionSet.Rotate},
                {InputName.Scroll, actionSet.Move},
                {InputName.UIVertical, actionSet.Move}
            };

            foreach (var entry in vector2XActionMap)
                entry.Value.onChange += (action, source, axis, delta) =>
                    TriggerCommand(entry.Key, axis.x);
            foreach (var entry in vector2YActionMap)
                entry.Value.onChange += (action, source, axis, delta) =>
                    TriggerCommand(entry.Key, axis.y);
            foreach (var entry in booleanActionMap)
                entry.Value.onChange += (action, source, state) =>
                {
                    if (!state) return;
                    TriggerCommand(entry.Key, 1);
                };
            foreach (var entry in invertedBooleanActionMap)
                entry.Value.onChange += (action, source, state) =>
                {
                    if (state) return;
                    TriggerCommand(entry.Key, 1);
                };
        }
        
        private static void TriggerCommand(string command, float axisValue)
        {
            if (!vgInputManager.Instance || vgInputManager.Instance.commandCallbackMap == null) return;

            if (!IsCommandAllowed(command)) return;

            var commandCallbackMap = vgInputManager.Instance.commandCallbackMap;
            if (!vgInputManager.Instance.flushCommands &&
                commandCallbackMap.TryGetValue(command, out var inputDelegate)) inputDelegate?.Invoke(axisValue);
        }

        private static bool IsCommandAllowed(string command)
        {
            foreach (var keyBind in vgInputManager.Instance.virtualKeyKeyBindMap.Values)
            foreach (var keyBindCommand in keyBind.commands)
                if (keyBindCommand.command == command)
                    return true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgAxisData), nameof(vgAxisData.Update))]
        private static void ReadAxisValuesFromSteamVR(vgAxisData __instance)
        {
            if (!SteamVR_Input.initialized) return;

            // TODO move this elsewhere.
            if (actionSet == null) Initialize();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            // TODO: could probably just use the streamingassets folder and avoid doing this?
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVrAssets/Bindings";
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.CheckForPCControls))]
        private static bool ForceDisablePcControls(vgPlayerController __instance)
        {
            __instance.minimumInputForJog = 0f;
            __instance.PCControlsActive = false;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.ForwardMovement))]
        private static bool FixForwardMovement(vgPlayerController __instance, float axisValue)
        {
            __instance.forwardInput = ProcessAxisValue(axisValue);
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.StrafeMovement))]
        private static bool FixStrafeMovement(vgPlayerController __instance, float axisValue)
        {
            __instance.strafeInput = ProcessAxisValue(axisValue);
            return false;
        }

        private const float outerDeadzone = 0.5f;
        private const float innerDeadzone = 0.1f;
        private static float ProcessAxisValue(float value)
        {
		    var valueSign = Mathf.Sign(value);
		    var absoluteValue = Mathf.Abs(value);
		    return valueSign * Mathf.InverseLerp(innerDeadzone, 1f - outerDeadzone, absoluteValue);
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgKeyBind), nameof(vgKeyBind.TriggerCommand))]
        private static bool SkipDefaultCommands()
        {
            return false;
        }
    }
}