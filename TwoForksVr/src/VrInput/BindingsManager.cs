using System.Collections.Generic;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput
{
    public class BindingsManager : MonoBehaviour
    {
        public static BindingsManager Instance; // TODO no singleton.
        private static SteamVR_Input_ActionSet_default actionSet;
        public Dictionary<string, IActionInput> ActionMap { get; private set; }

        public static BindingsManager Create(VrStage stage)
        {
            Instance = stage.gameObject.AddComponent<BindingsManager>();
            return Instance;
        }

        private void Awake()
        {
            Logs.LogInfo("## Initializing Bindings Patches");

            actionSet = SteamVR_Actions._default;
            ActionMap = new Dictionary<string, IActionInput>
            {
                {VirtualKey.LocomotionAction, ActionInputDefinitions.Interact},
                {VirtualKey.Camera, ActionInputDefinitions.Interact},
                {VirtualKey.Use, ActionInputDefinitions.Interact},
                {VirtualKey.Confirm, ActionInputDefinitions.Interact},
                {VirtualKey.StoreObject, ActionInputDefinitions.Jog},
                {VirtualKey.Cancel, ActionInputDefinitions.Cancel},
                {VirtualKey.DialogUp, ActionInputDefinitions.UIUp},
                {VirtualKey.DialogDown, ActionInputDefinitions.UIDown},
                // {InputName.UIUp, ActionInputDefinitions.UIUp},
                // {InputName.UIDown, ActionInputDefinitions.UIDown},
                // {InputName.LockNumberUp, ActionInputDefinitions.UIUp},
                // {InputName.LockNumberDown, ActionInputDefinitions.UIDown},
                // {InputName.LockTumblerRight, ActionInputDefinitions.NextPage},
                // {InputName.LockTumblerLeft, ActionInputDefinitions.PreviousPage},
                {VirtualKey.Jog, ActionInputDefinitions.Jog},
                {VirtualKey.Pause, ActionInputDefinitions.Cancel},
                {VirtualKey.NextMenu, ActionInputDefinitions.NextPage},
                {VirtualKey.PreviousMenu, ActionInputDefinitions.PreviousPage},
                {VirtualKey.Radio, ActionInputDefinitions.RadioUp},
                {VirtualKey.MoveXAxis, ActionInputDefinitions.MoveX},
                {VirtualKey.LookXAxisStick, ActionInputDefinitions.RotateX},
                {VirtualKey.UIRightStickHorizontal, ActionInputDefinitions.MoveX},
                {VirtualKey.MoveYAxis, ActionInputDefinitions.MoveY},
                {VirtualKey.LookYAxisStick, ActionInputDefinitions.RotateY},
                {VirtualKey.ScrollUpDown, ActionInputDefinitions.RotateY},
                // {InputName.DialogSelectionScroll, ActionInputDefinitions.RotateY},
                {VirtualKey.UIRightStickVertical, ActionInputDefinitions.MoveY}
            };
        }

        private void OnDestroy()
        {
            // foreach (var entry in Vector2XActionMap)
            //     entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            // foreach (var entry in Vector2YActionMap)
            //     entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            // foreach (var entry in BooleanActionMap)
            //     entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
            // foreach (var entry in InvertedBooleanActionMap)
            //     entry.Value.RemoveAllListeners(SteamVR_Input_Sources.Any);
        }

        public float GetValue(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return 0;

            return actionInput.Value;
        }

        public bool GetUp(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return false;

            return actionInput.ValueUp;
        }

        public bool GetDown(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return false;

            return actionInput.ValueDown;
        }

        private static void TriggerCommand(string virtualKey, float axisValue)
        {
            if (!vgInputManager.Instance || vgInputManager.Instance.virtualKeyKeyBindMap == null ||
                vgInputManager.Instance.commandCallbackMap == null) return;

            vgInputManager.Instance.virtualKeyKeyBindMap.TryGetValue(virtualKey, out var keyBind);
            if (keyBind == null) return;

            var commandCallbackMap = vgInputManager.Instance.commandCallbackMap;

            foreach (var keyBindCommand in keyBind.commands)
                if (!vgInputManager.Instance.flushCommands &&
                    commandCallbackMap.TryGetValue(keyBindCommand.command, out var inputDelegate))
                    inputDelegate?.Invoke(axisValue);
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