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
        public static SteamVR_Input_ActionSet_default ActionSet = SteamVR_Actions._default;
        public Dictionary<string, IActionInput> ActionMap { get; private set; }


        public static BindingsManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<BindingsManager>();
            return instance;
        }

        private void Awake()
        {
            Logs.LogInfo("## Initializing Bindings Patches");

            ActionMap = new Dictionary<string, IActionInput>
            {
                {VirtualKey.LocomotionAction, ActionInputDefinitions.Interact},
                {VirtualKey.Use, ActionInputDefinitions.Interact},
                {VirtualKey.Confirm, ActionInputDefinitions.Interact},
                {VirtualKey.StoreObject, ActionInputDefinitions.Jog},
                {VirtualKey.Cancel, ActionInputDefinitions.Cancel},
                {VirtualKey.DialogUp, ActionInputDefinitions.UIUp},
                {VirtualKey.DialogDown, ActionInputDefinitions.UIDown},
                {VirtualKey.Jog, ActionInputDefinitions.Jog},
                {VirtualKey.Pause, ActionInputDefinitions.Cancel},
                {VirtualKey.NextMenu, ActionInputDefinitions.NextPage},
                {VirtualKey.PreviousMenu, ActionInputDefinitions.PreviousPage},
                {VirtualKey.Radio, ActionInputDefinitions.Radio},
                {VirtualKey.MoveXAxis, ActionInputDefinitions.MoveX},
                {VirtualKey.LookXAxisStick, ActionInputDefinitions.RotateX},
                {VirtualKey.MoveYAxis, ActionInputDefinitions.MoveY}
            };
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
    }
}