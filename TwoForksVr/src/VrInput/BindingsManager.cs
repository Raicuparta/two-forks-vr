using System.Collections.Generic;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput
{
    public class BindingsManager : MonoBehaviour
    {
        private static SteamVR_Input_ActionSet_default actionSet;
        public Dictionary<string, SteamVR_Action_Boolean> BooleanActionMap { get; private set; }
        public Dictionary<string, SteamVR_Action_Boolean> InvertedBooleanActionMap { get; private set; }
        public Dictionary<string, SteamVR_Action_Vector2> Vector2XActionMap { get; private set; }
        public Dictionary<string, SteamVR_Action_Vector2> Vector2YActionMap { get; private set; }

        public static BindingsManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<BindingsManager>();
            return instance;
        }

        private void Awake()
        {
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
                {InputName.Scroll, actionSet.Rotate},
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

        private void OnDestroy()
        {
            foreach (var entry in Vector2XActionMap)
                entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            foreach (var entry in Vector2YActionMap)
                entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            foreach (var entry in BooleanActionMap)
                entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            foreach (var entry in InvertedBooleanActionMap)
                entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
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
    }
}