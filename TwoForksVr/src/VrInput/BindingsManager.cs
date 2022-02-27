using System.Collections.Generic;
using BepInEx.Configuration;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput
{
    public class BindingsManager : MonoBehaviour
    {
        public readonly Dictionary<string, IActionInput> ActionMap = new Dictionary<string, IActionInput>
        {
            {VirtualKey.LocomotionAction, ActionInputDefinitions.LocomotionAction},
            {VirtualKey.Use, ActionInputDefinitions.Interact},
            {VirtualKey.Confirm, ActionInputDefinitions.Interact},
            {VirtualKey.StoreObject, ActionInputDefinitions.StoreItem},
            {VirtualKey.Cancel, ActionInputDefinitions.Cancel},
            {VirtualKey.DialogUp, ActionInputDefinitions.UiUp},
            {VirtualKey.DialogDown, ActionInputDefinitions.UiDown},
            {VirtualKey.Jog, ActionInputDefinitions.Jog},
            {VirtualKey.Pause, ActionInputDefinitions.Cancel},
            {VirtualKey.NextMenu, ActionInputDefinitions.NextPage},
            {VirtualKey.PreviousMenu, ActionInputDefinitions.PreviousPage},
            {VirtualKey.Radio, ActionInputDefinitions.Radio},
            {VirtualKey.MoveXAxis, ActionInputDefinitions.MoveX},
            {VirtualKey.LookXAxisStick, ActionInputDefinitions.RotateX},
            {VirtualKey.MoveYAxis, ActionInputDefinitions.MoveY},
            {VirtualKey.MoveForwardKeyboard, ActionInputDefinitions.UiUp},
            {VirtualKey.MoveBackwardKeyboard, ActionInputDefinitions.UiDown},
            {VirtualKey.StrafeRightKeyboard, ActionInputDefinitions.NextPage},
            {VirtualKey.StrafeLeftKeyboard, ActionInputDefinitions.PreviousPage},

            // Unused for actually controlling stuff, but used for the input prompts.
            {VirtualKey.ScrollUpDown, ActionInputDefinitions.UiUp},
            {VirtualKey.ToolPicker, ActionInputDefinitions.ToolPicker},
            {VirtualKey.Inventory, ActionInputDefinitions.Cancel}
        };

        public static BindingsManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<BindingsManager>();
            return instance;
        }

        private void Awake()
        {
            ActivateSteamVrActionSets();
        }

        private void OnEnable()
        {
            SteamVR_Events.System(EVREventType.VREvent_Input_BindingsUpdated).Listen(HandleVrBindingsUpdated);
            VrSettings.Config.SettingChanged += HandleSettingChanged;
        }

        private void OnDisable()
        {
            SteamVR_Events.System(EVREventType.VREvent_Input_BindingsUpdated).Remove(HandleVrBindingsUpdated);
            VrSettings.Config.SettingChanged -= HandleSettingChanged;
        }

        private void ActivateSteamVrActionSets()
        {
            foreach (var actionSet in SteamVR_Input.actionSets) actionSet.Activate();
        }

        private static void HandleSettingChanged(object sender, SettingChangedEventArgs e)
        {
            UpdatePrompts();
        }

        private static void HandleVrBindingsUpdated(VREvent_t arg0)
        {
            UpdatePrompts();
        }

        private static void UpdatePrompts()
        {
            if (!vgInputManager.Instance) return;

            // This resets the input prompts. The layout choice argument isn't actually used.
            vgInputManager.Instance.SetControllerLayout(vgControllerLayoutChoice.KeyboardMouse);
        }

        public float GetValue(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return 0;

            return actionInput.AxisValue;
        }

        public bool GetUp(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return false;

            return actionInput.ButtonUp;
        }

        public bool GetDown(string virtualKey)
        {
            ActionMap.TryGetValue(virtualKey, out var actionInput);
            if (actionInput == null) return false;

            return actionInput.ButtonDown;
        }
    }
}