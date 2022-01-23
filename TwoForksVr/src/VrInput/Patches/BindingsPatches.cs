using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using TwoForksVr.UI;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public static class BindingsPatches
    {
        private static bool isInitialized;
        private static SteamVR_Input_ActionSet_default actionSet;
        public static Dictionary<string, SteamVR_Action_Boolean> BooleanActionMap { get; private set; }
        public static Dictionary<string, SteamVR_Action_Boolean> InvertedBooleanActionMap { get; private set; }
        public static Dictionary<string, SteamVR_Action_Vector2> Vector2XActionMap { get; private set; }
        public static Dictionary<string, SteamVR_Action_Vector2> Vector2YActionMap { get; private set; }

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;
            
            Logs.LogInfo("## Initializing Bindings Patches");
            
            actionSet = SteamVR_Actions._default;
            BooleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
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
            InvertedBooleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>
            {
                {InputName.RadioDown, actionSet.Grip}
            };
            Vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveStrafe, actionSet.Move},
                {InputName.LookHorizontalStick, actionSet.Rotate},
                {InputName.UIHorizontal, actionSet.Move}
            };
            Vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>
            {
                {InputName.MoveForward, actionSet.Move},
                {InputName.LookVerticalStick, actionSet.Rotate},
                {InputName.Scroll, actionSet.Move},
                {InputName.UIVertical, actionSet.Move}
            };

            foreach (var entry in Vector2XActionMap)
                entry.Value.onChange += (action, source, axis, delta) =>
                    TriggerCommand(entry.Key, axis.x);
            foreach (var entry in Vector2YActionMap)
                entry.Value.onChange += (action, source, axis, delta) =>
                    TriggerCommand(entry.Key, axis.y);
            foreach (var entry in BooleanActionMap)
                entry.Value.onChange += (action, source, state) =>
                {
                    if (!state) return;
                    TriggerCommand(entry.Key, 1);
                };
            foreach (var entry in InvertedBooleanActionMap)
                entry.Value.onChange += (action, source, state) =>
                {
                    if (state) return;
                    TriggerCommand(entry.Key, 1, true);
                };
        }

        private static void TriggerCommand(string command, float axisValue, bool forceAllowed = false)
        {
            if (!vgInputManager.Instance || vgInputManager.Instance.commandCallbackMap == null) return;

            if (!forceAllowed && !IsCommandAllowed(command)) return;

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
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            // TODO: could probably just use the streamingassets folder and avoid doing this?
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVrAssets/Bindings";
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgRewiredInput), nameof(vgRewiredInput.UpdateActiveController))]
        private static bool ForceXboxController(vgRewiredInput __instance)
        {
            __instance.activeController = vgControllerLayoutChoice.XBox;
            __instance.mCurrentIconMap = __instance.IconMap_Xbox;
            __instance.mCurrentIconMap.Init();

            if (vgSettingsManager.Instance &&
                vgSettingsManager.Instance.controller != (int) __instance.activeController)
                vgSettingsManager.Instance.controller = (int) __instance.activeController;

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
            var processedInput = ProcessAxisValue(axisValue);
            if (VrSettings.Teleport.Value)
            {
                // Prevent walking backwards if teleport mode is on.
                processedInput = Mathf.Max(0, processedInput);
            }
            __instance.forwardInput = processedInput;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.StrafeMovement))]
        private static bool FixStrafeMovement(vgPlayerController __instance, float axisValue)
        {
            if (VrSettings.Teleport.Value) return false;

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
        private static bool IgnoreDefaultAxisInputs(string command)
        {
            return !Vector2XActionMap.ContainsKey(command) && !Vector2YActionMap.ContainsKey(command);
        }
    }
}